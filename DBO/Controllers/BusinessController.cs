using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DBO.Data;
using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Data.Utilities;
using DBO.Data.ViewModels;
using DBO.Extensions;
using DBO.Services.Email;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;

using static DBO.Common.Constants;

namespace DBO.Controllers
{
    [AllowAnonymous]
    public class BusinessController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly CompanyRepository _companyRepository = new CompanyRepository();
        private readonly ConnectionRepository _connectionRepository = new ConnectionRepository();
        private readonly FollowerRepository _followerRepository = new FollowerRepository();
        private readonly RegistrationRepository _registrationRepository = new RegistrationRepository();
        private readonly UserRepository _userRepository = new UserRepository();

        // GET: Companies/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get company with id
            var company = _companyRepository.GetCompany(id.Value);
            if (company == null)
            {
                return HttpNotFound();
            }

            // get connections
            var connections = _connectionRepository.Query().Include(nameof(Connection.Company1))
                                                           .Include(nameof(Connection.Company2))
                                                           .Where(x => x.CompanyId1 == id || x.CompanyId2 == id).ToList();

            var usersCompany = GetUserCompany();

            // make model for view
            var model = new CompanyViewModel(company)
            {
                Skills = db.CompanySkills.Include(s => s.Skill).Where(s => s.CompanyId == id).ToList(),
                ApprovedConnections = connections.Where(x => x.Status == ConnectionStatus.Approved).Select(x => x.CompanyId1 == id ? x.Company2 : x.Company1).ToList(),
                PendingConnections = connections.Where(x => x.Status == ConnectionStatus.Requested && x.CompanyId1 == id).Select(x => x.CompanyId1 == id ? x.Company2 : x.Company1).ToList(),
                Followers = _followerRepository.Query().Count(x => x.CompanyId == id),
                CurrentCompany = usersCompany != null ? Convert.ToInt32(usersCompany) : -1
            };

            // menage viewbag fields
            ViewBag.Success = Session["ClaimSuccess"]?.ToString();
            Session["ClaimSuccess"] = string.Empty;
            ViewBag.GoogleMapsApiKey = ConfigurationManager.AppSettings["GOOGLEMAPS_API_KEY"];

            // get best available address
            ViewBag.AddressForGoogleApi = this.GetBestAddress(model.Address2, model.Address1);

            // if company have no user, add clime opportunity
            var companyUser = await GetCompanyUser(id);
            if (companyUser == null)
            {
                return View("ClaimingDetails", new ClaimingViewModel { Company = model });
            }

            return View(model);
        }

        [Authorize]
        public string GetUserCompany()
        {
            // get user manager
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // get user
            var userId = User.Identity.GetUserId();
            var user = userId != null ? userManager.FindById(userId) : null;

            if (user == null) return null;

            return user.CompanyId != null ? user.CompanyId.Value.ToString() : null;
        }

        [HttpGet]
        public ActionResult ClaimCompany(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var company = _companyRepository.GetCompanyWithDetails(id.Value);

            if (company == null)
            {
                return HttpNotFound();
            }

            var model = new ClaimingViewModel
            {
                Company = new CompanyViewModel(company)
            };

            return PartialView("_ClaimCompany", model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ClaimCompany([Bind(Exclude = nameof(ClaimingViewModel.Name))]ClaimingViewModel model)
        {
            ModelState.Remove(nameof(ClaimingViewModel.Name));

            if (ModelState.IsValid)
            {
                // get user
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = await userManager.FindByNameAsync(model.Email);

                if (IsValidDomain(model.Company.Email, model.Email) && user == null)
                {
                    //if user doesn't exist, create new one
                    var password = $"{Guid.NewGuid().ToString().Substring(0, 8)}!1kR";
                    user = new ApplicationUser { UserName = model.Email, Email = model.Email, CompanyId = model.Company.Id };

                    // create user
                    if (await userManager.CreateAsync(user, password) == IdentityResult.Success)
                    {
                        // add user role
                        if (userManager.AddToRole(user.Id, CompanyRole) == IdentityResult.Success)
                        {
                            // login user
                            await HttpContext.GetOwinContext().Get<ApplicationSignInManager>()
                                    .SignInAsync(user, isPersistent: false, rememberBrowser: false);

                            // send password via email
                            this.SendEmail(model, password);

                            return Json(new { RedirectUrl = Url.Action("BasicData", "Home", new { id = model.Company.Id }) });
                        }
                    }
                }
                else
                {
                    // if user exists, make a clime request
                    await _registrationRepository.AddRegistrationRequest(new ClaimRequest
                    {
                        CompanyId = model.Company.Id,
                        Email = model.Email,
                        RequestTime = DateTime.Now
                    });
                }

                Session["ClaimSuccess"] = ResourceString.Instance.ClaimCompany_Success;
                return Json(new { success = true });
            }

            return PartialView("_ClaimCompany", model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ClaimCompanyReferal([Bind(Exclude = nameof(ClaimingViewModel.IsAgreedWithTerms))]ClaimingViewModel model, string partialName)
        {
            ModelState.Remove(nameof(ClaimingViewModel.IsAgreedWithTerms));
            if (ModelState.IsValid)
            {
                if (IsValidDomain(model.Company.Email, model.Email))
                {
                    var bodyHtml = System.IO.File.ReadAllText(Server.MapPath("/Templates/ClaimCompanyReferal.html"));
                    var bodyBuilder = new StringBuilder(bodyHtml);

                    var signupLink = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}{Url.Action(nameof(Details), "Business", new { id = model.Company.Id })}";

                    bodyBuilder.Replace("@@CompanyName@@", model.Company.Name);
                    bodyBuilder.Replace("@@Address@@", signupLink);
                    bodyBuilder.Replace("@@Name@@", model.Name);

                    var emailService = new GoogleEmailService(model.Email, "Claim company", model.Email, bodyBuilder.ToString(), true, false);
                    emailService.SendMail();
                }
                else
                {
                    await _registrationRepository.AddRegistrationRequest(new ClaimRequest
                    {
                        CompanyId = model.Company.Id,
                        Email = model.Email,
                        RequestTime = DateTime.Now
                    });
                }

                ViewBag.SuccessMessage = ResourceString.Instance.RegistrationSuccessfulMessage;
            }

            return PartialView(partialName, model);
        }

        private async Task<ApplicationUser> GetCompanyUser(int? companyId)
        {
            var companyUsers = _userRepository.GetUsersByRole(Common.Constants.CompanyRole);
            var user = companyUsers.FirstOrDefault(x => x.CompanyId == companyId);
            return user;
        }

        /// <summary>
        /// Get best available address
        /// </summary>
        private string GetBestAddress(string city, string street)
        {
            // generate address variations
            string[] addresArray = new[] { city + "," + street, city };

            // search best available address
            foreach (var address in addresArray)
            {
                // get result from google maps api
                string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + address + "&key=" + ConfigurationManager.AppSettings["GOOGLEMAPS_API_KEY"];
                object getResult = new WebClient().DownloadString(url);
                JObject parseObj = JObject.Parse(getResult.ToString());

                // if address exists, return it
                if (parseObj.GetValue("status").ToString() != "ZERO_RESULTS")
                {
                    return address;
                }
            }

            return null;
        }

        /// <summary>
        /// Send email with new password
        /// </summary>
        private void SendEmail(ClaimingViewModel model, string password)
        {
            // send an email
            var bodyHtml = System.IO.File.ReadAllText(Server.MapPath("/Templates/ClaimCompany.html"));
            var bodyBuilder = new StringBuilder(bodyHtml);

            var loginLink = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/{Url.Action("Details", "Business", new { id = model.Company.Id })}";


            bodyBuilder.Replace("@@CompanyName@@", model.Company.Name);
            bodyBuilder.Replace("@@Address@@", loginLink);
            bodyBuilder.Replace("@@Password@@", password);

            var emailService = new GoogleEmailService(model.Email, "Claim company", model.Email, bodyBuilder.ToString(), true, false);
            emailService.SendMail();
        }

        private bool IsValidDomain(string companyEmail, string email)
        {
            var patern = @"(?<=@)[^.]+(?=\.)";
            var domainName = Regex.Match(email, patern).Value;
            var companyDomainName = !string.IsNullOrEmpty(companyEmail) ? Regex.Match(companyEmail, patern).Value : string.Empty;

            return string.IsNullOrEmpty(companyDomainName) || domainName.Equals(companyDomainName, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}