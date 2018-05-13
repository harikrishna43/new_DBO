using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Data.ViewModels;
using DBO.Extensions;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Constants = DBO.Common.Constants;
using DBO.Services.Contract;
using System.Collections.Generic;

namespace DBO.Controllers
{
    public class HomeController : BaseController
    {
        private readonly CompanyRepository _companyRepository = new CompanyRepository();
        private readonly ConnectionRepository _connectionsRepository = new ConnectionRepository();
        private readonly CompanySkillRepository _companySkillRepository = new CompanySkillRepository();
        private readonly NewsRepository _newsRepository = new NewsRepository();
        private readonly SkillRepository _skillRepository = new SkillRepository();
        private readonly LanguageRepository _languageRepository = new LanguageRepository();
        private readonly NotificationSettingsRepository _notificationSettingsRepository = new NotificationSettingsRepository();
        private INotificationService _notificationService;

        public HomeController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public ActionResult Index(string search, string searchCity)
        {
            var companies = _companyRepository.GetCompanies(search, searchCity, out var hasMore);

            ViewBag.Search = search;
            ViewBag.SearchCity = searchCity;
            ViewBag.HasMoreResults = hasMore;
            return View(companies);
        }

        public ActionResult ReadMore(string search, string searchCity, int pageNumber)
        {
            ViewBag.Search = search;
            ViewBag.SearchCity = searchCity;
            var companies = _companyRepository.GetCompanies(search, searchCity, out var hasMore, pageNumber);
            Response.Headers["X-HasMoreResults"] = hasMore.ToString().ToLower();

            return PartialView("_ReadMoreCompaniesPartial", companies);
        }

        public ActionResult NewsFeed()
        {
            var userId = User.GetClaimValue(Constants.UserIdClaim);
            var companyId = User.GetClaimValue(Constants.CompanyIdClaim);
            var model = _newsRepository.GetNewsFeed(userId, companyId, out var hasMore, isAdmin: User.IsInRole(Constants.AdminRole));
            var role = User.GetClaimValue(x => x.Type.Contains("role"));

            ViewBag.HasMoreResults = hasMore;
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = !string.IsNullOrEmpty(companyId)
                ? _companyRepository.GetCompany(int.Parse(companyId))?.Name
                : string.Empty;
            ViewBag.CanWriteNewsfeed = !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(companyId) && !role.Equals(Constants.AdminRole);

            return View(model);
        }

        public ActionResult ReadMoreNews(int pageNumber)
        {
            var userId = User.GetClaimValue(Constants.UserIdClaim);
            var companyId = User.GetClaimValue(Constants.CompanyIdClaim);
            var model = _newsRepository.GetNewsFeed(userId, companyId, out var hasMore, pageNumber, User.IsInRole(Constants.AdminRole));
            Response.Headers["X-HasMoreResults"] = hasMore.ToString().ToLower();

            return PartialView("~/Views/News/News.cshtml", model);
        }

        public async Task<ActionResult> SetCulture(string languageName)
        {
            var language = await _languageRepository.GetByName(languageName);

            if (language == null)
            {
                languageName = Constants.EnglishLanguage;
            }

            // Save language in a cookie
            HttpCookie cookie = Request.Cookies[Constants.CookieLanguage];

            if (cookie != null)
            {
                cookie.Value = languageName;
            }
            else
            {
                cookie = new HttpCookie(Constants.CookieLanguage)
                {
                    Value = languageName,
                    Expires = DateTime.Now.AddYears(1)
                };
            }

            Response.Cookies.Add(cookie);
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        public ActionResult Register(string token)
        {
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(token)) return Redirect("Home/Index");
            if (string.IsNullOrEmpty(token)) return View();

            if (!Guid.TryParse(token, out var guid)) return View();

            var registrationRepository = new RegistrationRepository();
            var companyId = registrationRepository.GetCompanyByToken(guid);
            if (companyId == null) return View();

            var company = _companyRepository.GetCompany(companyId.Value);
            var model = new BaseCompanyInfoViewModel
            {
                CompanyId = company.Id,
                Name = company.Name,
                Cvr = company.CVR.ToString(),
                Email = company.Email,
                Token = guid
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Register(BaseCompanyInfoViewModel vm)
        {
            ViewBag.Role = vm.Role;
            bool isValid = true;
            int cvr = 0;
            if (string.IsNullOrEmpty(vm.Name))
            {
                ModelState.AddModelError("", "Name is required");
                isValid = false;
            }

            if (string.IsNullOrEmpty(vm.Email))
            {
                ModelState.AddModelError("", "Email is required");
                isValid = false;
            }

            if (vm.Role == Constants.CompanyRole && (string.IsNullOrEmpty(vm.Cvr) || !int.TryParse(vm.Cvr, out cvr)))
            {
                ModelState.AddModelError("", "CVR is not valid");
                isValid = false;
            }

            //var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var emailInUse = await _companyRepository.EmailInUseByOtherCompany(vm.Email, vm.CompanyId);
            //if (!string.IsNullOrEmpty(vm.Email) && (userManager.FindByName(vm.Email) != null || emailInUse))
            //{
            //    ModelState.AddModelError("", "This email is already in use");
            //    isValid = false;
            //}

            if (isValid)
            {
                var registrationRepository = new RegistrationRepository();
                if (vm.Role == Constants.CompanyRole)
                {
                    registrationRepository.AddCompanyRegistration(vm.Token, cvr, vm.Name, vm.Email);
                }
                else
                {
                    registrationRepository.AddPersonRegistration(vm.Name, vm.Email);
                }

                return RedirectToAction("Thanks", "Home");

                //Company company;
                //if (vm.CompanyId <= 0)
                //{
                //    company = new Company
                //    {
                //        Email = vm.Email,
                //        CVR = cvr,
                //        Name = vm.Name
                //    };

                //    _companyRepository.Add(company);
                //    return RedirectToAction("Thanks", "Home");
                //}

                //company =  _companyRepository.GetThreeCompanies(vm.CompanyId);
                //var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
                //var password = Guid.NewGuid().ToString().Substring(0, 8) + "!1jK";
                //var result = await userManager.CreateAsync(user, password);

                //if (result.Succeeded)
                //{
                //    await userManager.AddToRoleAsync(user.Id, Constants.CompanyRole);

                //    company.Name = vm.Name;
                //    company.Email = vm.Email;
                //    company.CVR = cvr;
                //    await _companyRepository.UpdateCompany(company);

                //    user.CompanyId = company.Id;
                //    await userManager.UpdateAsync(user);

                //    var subject = "Company registered";
                //    var body = "Your company is registered. Your account password is " + password;

                //    var emailService = new EmailService(
                //        company.Email,
                //        subject,
                //        company.Name,
                //        body,
                //        true,
                //        false
                //    );
                //    emailService.Send();

                //    await userManager.AddClaimAsync(user.Id, new Claim(Common.Constants.UserIdClaim, user.Id));
                //    await userManager.AddClaimAsync(user.Id,
                //        new Claim(Common.Constants.CompanyIdClaim, user.CompanyId.ToString()));
                //    await HttpContext.GetOwinContext().Get<ApplicationSignInManager>()
                //        .SignInAsync(user, isPersistent: false, rememberBrowser: false);

                //    return RedirectToAction("BasicData", "Home",
                //        new
                //        {
                //            id = user.CompanyId,
                //            message =
                //            "Thank you for registre your company, please take some time to fill out your company profile."
                //        });
                //}

                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError("", error);
                //}
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> BasicData(int id)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var userId = User.GetClaimValue(Constants.UserIdClaim);

            //if (await userManager.IsInRoleAsync(userId, Constants.AdminRole) ||
            //(int.TryParse(User.GetClaimValue(Constants.CompanyIdClaim), out int userCompanyId) && await userManager.IsInRoleAsync(userId, Constants.CompanyRole) && userCompanyId == id))
            //{
            var company = _companyRepository.GetCompanyWithDetails(id);
            var connections = _connectionsRepository.Query().Include(nameof(Connection.Company2)).Where(x => x.CompanyId1 == id || x.CompanyId2 == id).ToList();
            int.TryParse(User.GetClaimValue(Constants.CompanyIdClaim), out var currentCompany);

            var model = new CompanyViewModel(company)
            {
                Skills = _companySkillRepository.GetSkills(company.Id),
                ApprovedConnections = connections.Where(x => x.Status == ConnectionStatus.Approved).Select(x => x.CompanyId1 == id ? x.Company2 : x.Company1).ToList(),
                PendingConnections = connections.Where(x => x.Status == ConnectionStatus.Requested && x.CompanyId1 == id).Select(x => x.CompanyId1 == id ? x.Company2 : x.Company1).ToList(),
                CurrentCompany = currentCompany,
                Employees = company.Employees,
                NotificationSettings = await _notificationSettingsRepository.GetByUserId(userId)
            };

            ViewBag.Skills = _skillRepository.GetAll()
                                             .Where(x => company.CompanySkills.All(y => y.SkillId != x.Id));

            if (model.NotificationSettings == null)
            {
                model.NotificationSettings = await _notificationSettingsRepository.Add(new NotificationSettings
                {
                    UserId = userId,
                    NotificationIteration = NotificationIteration.WithoutNotification
                });
            }
            ViewBag.GoogleMapsApiKey = ConfigurationManager.AppSettings["GOOGLEMAPS_API_KEY"];

            return View(model);
            //}
            //else
            //{
            //    return new HttpUnauthorizedResult();
            //}
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> UpdateAddress(CompanyViewModel model)
        {
            await _companyRepository.UpdateAddress(model.Id, model.City, model.Address, model.PostCode);
            return RedirectToAction(nameof(BasicData), new { id = model.Id });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> UpdateImage(CompanyViewModel model, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                await _companyRepository.UploadLogo(model.Id, file);
            }

            return RedirectToAction(nameof(BasicData), new { id = model.Id });
        }

        [HttpPost]
        [Authorize]
        public async Task UpdateNotificationSettings(NotificationSettingsViewModel settingsVm)
        {
            var mapper = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<NotificationSettingsViewModel, NotificationSettings>()).CreateMapper();
            var settingsToUpdate = mapper.Map<NotificationSettingsViewModel, NotificationSettings>(settingsVm);
            await _notificationSettingsRepository.Update(settingsToUpdate);
        }

        public ActionResult CheckForNotifications()
        {
            var userId = User.GetClaimValue(Constants.UserIdClaim);
            var notificationsForBells = new List<NotificationViewModel>();
            if (CurrentUserCompanyId != 0)
                notificationsForBells = _notificationService.GetAllBellsNotifications(CurrentUserCompanyId, userId);
            return PartialView("_NotificationBellPartial", notificationsForBells);
        }

        [HttpPost]
        [Authorize]
        public void ProcessNotifications(IEnumerable<int> notificationIds)
        {
            if (notificationIds?.Any() == true)
            {
                var userId = User.GetClaimValue(Constants.UserIdClaim);
                _notificationService.ProcessNotifications(userId, notificationIds);
            }
        }


        public ActionResult Thanks()
        {
            return View();
        }

        public ActionResult JobExchange()
        {
            return View();
        }
    }
}