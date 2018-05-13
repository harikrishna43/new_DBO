using DBO.Common;
using DBO.Data;
using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Data.Utilities;
using DBO.Data.ViewModels;
using DBO.Extensions;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using static DBO.Common.Constants;

namespace DBO.Controllers
{
    [Authorize(Roles = AdminRole)]
    public class CompaniesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly CompanyRepository _companyRepository = new CompanyRepository();
        private readonly SkillRepository _skillRepository = new SkillRepository();
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly CompanySkillRepository _companySkillRepository = new CompanySkillRepository();

        public ActionResult Index(SearchParams search)
        {
            if (TempData["DeleteSearchParams"] != null)
            {
                search = (SearchParams)TempData["DeleteSearchParams"];
                TempData["DeleteSearchParams"] = null;
            }
            var companies = _companyRepository.GetCompanySearch(search, out var hasMore, out var count);

            ViewBag.HasMoreResults = hasMore;
            ViewBag.Count = count;
            ViewBag.IndustryList = _companyRepository.GetIndustryList();

            var model = new AdminSearchViewModel
            {
                Companies = companies,
                SearchParams = search
            };

            return View(model);
        }

        public ActionResult ReadMore(SearchParams search, int page)
        {
            var companies = _companyRepository.GetCompanySearch(search, out var hasMore, out var count, page);
            Response.Headers["X-HasMoreResults"] = hasMore.ToString().ToLower();

            return PartialView("_ReadMoreCompaniesPartial", companies);
        }

        private async Task<ApplicationUser> GetCompanyUser(int? companyId)
        {
            var companyUsers = _userRepository.GetUsersByRole(Common.Constants.CompanyRole);
            var user = companyUsers.FirstOrDefault(x => x.CompanyId == companyId);
            return user;
        }

        // GET: Companies/Admin/5
        [HttpGet]
        public async Task<ActionResult> Edit(int? id, SearchParams searchParams, int? n)
        {
            if ((id == null || id == -1) && n == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var three = n == null ? null : _companyRepository.GetCompany(searchParams, n.Value);
            if (id == null)
            {
                id = three.Current;
                if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Company company = _companyRepository.GetCompanyWithDetails(id.Value);
            if (company == null)
            {
                return HttpNotFound();
            }

            PrepareCompanyViewBag(company, n);
            var model = new AdminCompanyViewModel
            {
                Company = company,
                CompanyNavigation = three,
                SearchParams = searchParams,
                IsClaimed = await GetCompanyUser(id) != null
            };

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminCompanyViewModel model, SearchParams searchParams, int? n)
        {
            if (ModelState.IsValid)
            {
                await _companyRepository.UpdateCompany(model.Company);
            }
            else
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in allErrors)
                {
                    ViewBag.Message = error.ErrorMessage + "\r\n";
                }
            }

            if (model.Company.IndustryId <= 0) model.Company.IndustryId = null;
            model.Company = _companyRepository.GetCompanyWithDetails(model.Company.Id);
            model.SearchParams = searchParams;
            PrepareCompanyViewBag(model.Company, n);

            return View(model);
        }

        private void PrepareCompanyViewBag(Company company, int? n)
        {
            if (n != null) ViewBag.N = n.Value;

            ViewBag.IndustryList = _companyRepository.GetIndustryList();
            ViewBag.IndustryIdList = _companyRepository.GetIndustryList(company.IndustryId);

            var template = GetEmailContent(company);
            ViewBag.EmailText = template;

            ViewBag.Skills = _skillRepository.GetAll().Where(x => company.CompanySkills.All(y => y.SkillId != x.Id));

            ViewBag.Message = TempData["Message"];

            var skills = db.Skills.ToList();
            skills.Insert(0, new Skill { Id = -1, Name = "---" });
            ViewBag.Skill = new SelectList(skills, "Id", "Name");
        }

        private StringBuilder GetEmailContent(Company company)
        {
            var template = new StringBuilder(System.IO.File.ReadAllText(Server.MapPath("/Templates/IntroEmail.html")));
            var name = company.PersonName;
            var parts = name?.Split(' ') ?? new[] { "" };
            var hostName = string.Empty;

            if ((System.Web.HttpContext.Current != null))
            {
                Uri uri = new Uri(System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
                string pathQuery = uri.PathAndQuery;
                hostName = uri.ToString().Replace(pathQuery, string.Empty);
                Session["HostName"] = hostName;
            }

            if (string.IsNullOrEmpty(hostName) && Session["HostName"] != null)
            {
                hostName = Session["HostName"] as string;
            }

            ViewBag.Header = $"{hostName}/img/bg-email.jpg";
            ViewBag.Logo = $"{hostName}/img/dbo.png";

            if (parts.Length >= 2) name = parts[0];
            template.Replace("@@User@@", name);
            return template;
        }

        [HttpPost]
        public ActionResult RemoveCompanySkill(int skillId, int companyId)
        {
            var company = _companyRepository.GetCompanyWithDetails(companyId);
            var skill = _skillRepository.Get(skillId);

            if (company == null || skill == null)
            {
                return HttpNotFound();
            }

            _companySkillRepository.Remove(companyId, skillId);
            return Json(string.Empty);
        }

        [HttpPost]
        public ActionResult AddCompanySkill(int? skillId, int companyId, string skillName)
        {
            var company = _companyRepository.GetCompanyWithDetails(companyId);
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var userId = User.GetClaimValue(UserIdClaim);
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.Identity.GetUserId();
            }

            if (userManager.IsInRole(userId, CompanyRole) && company.CompanySkills.Count >= 3)
            {
                return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed, ResourceString.Instance.SkillsLimitReached);
            }

            Skill skill = null;
            if (skillId == null)
            {
                skill = new Skill { Name = skillName };
                _skillRepository.Add(skill);
            }
            else
            {
                skill = _skillRepository.Get(skillId.Value);
            }

            if (company == null || skill == null)
            {
                return HttpNotFound();
            }

            _companySkillRepository.Add(new CompanySkill
            {
                SkillId = skill.Id,
                CompanyId = companyId
            });
            return Json(new { skillId = skill.Id });
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id, bool fromIndex = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _companyRepository.GetCompanyWithDetails(id.Value);
            if (company == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (fromIndex)
                {
                    _companyRepository.Delete(company);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    _companyRepository.Delete(company);
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, SearchParams search)
        {
            Company company = db.Companies.Find(id);
            if (company != null)
            {
                db.Companies.Remove(company);
                db.SaveChanges();
            }

            TempData["DeleteSearchParams"] = search;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Employments()
        {
            var employees = _userRepository.GetUsersByRole(EmployeeRole);
            return View(employees);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateEmployment(string username, int? companies)
        {
            if (string.IsNullOrEmpty(username))
            {
                return View("Error");
            }

            var user = await _userRepository.FindAsync(username);
            user.CompanyId = companies;

            await _userRepository.SaveAsync();
            return RedirectToAction(nameof(Employments));
        }

        public ActionResult CompanySelect2(int pageSize, int page, string term)
        {
            var model = _companyRepository.GetSelect2(pageSize, page, term);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IndustryCodeSelectTwo(int pageSize, int page, string term)
        {
            var model = _companyRepository.GetSelect2IndustryCodes(pageSize, page, term);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<ActionResult> SignInAs(int id)
        {
            var user = await GetCompanyUser(id);
            //var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var authenticationManager = HttpContext.GetOwinContext().Authentication;

            ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaimsForUser(user, CompanyRole);

            authenticationManager.SignIn(identity);

            if (Session["ReturnToAdminRole"] == null)
                Session["ReturnToAdminRole"] = true;

            return RedirectToAction("Details", "Business", new { id });
        }
    }
}
