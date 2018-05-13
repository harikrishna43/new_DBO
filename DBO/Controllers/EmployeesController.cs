using DBO.Common;
using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Data.Repositories.Contract;
using DBO.Data.ViewModels;
using DBO.Extensions;
using DBO.Services.Email;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DBO.Controllers
{
    public class EmployeesController : BaseController
    {
        private readonly CompanyRepository companyRepository;
        private ApplicationUserManager userManager;
        private UserRepository usersRepository;

        public EmployeesController()
        {
            companyRepository = new CompanyRepository();
            usersRepository = new UserRepository();
        }


        public ApplicationUserManager UserManager
        {
            get => userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => userManager = value;
        }

        [HttpGet]
        [Authorize]
        public ActionResult List(int companyId)
        {
            var company = companyRepository.GetCompanyWithDetails(companyId);
            var employees = company.Employees.Where(e => e.Roles.Any(r => r.RoleId == Constants.EmployeeRoleId));
            ViewBag.CompanyId = companyId;
            return PartialView("List", employees);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create(int companyId)
        {
            var company = companyRepository.GetCompany(companyId);
            var newEmployee = new EmployeeViewModel { CompanyId = companyId, CompanyName = company?.Name };
            return PartialView("Create", newEmployee);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Create", model);
            }

            var user = new ApplicationUser { UserName = model.Name, Email = model.Email, Title = model.Title, CompanyId = model.CompanyId };
            var password = Guid.NewGuid().ToString().Substring(0, 8) + "!1jK";
            var result = await UserManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var directoryPath = Server.MapPath(DBO.Common.Constants.UserImagePath);
                usersRepository.CreateUsersFile(user.Id, model.FilePath, FileType.Image, directoryPath);
                await UserManager.AddToRoleAsync(user.Id, Constants.EmployeeRole);
                string subject = $"A {model.CompanyName} emloyee was created";
                string body = "Your emloyee account is registered. Your password is " + password;
                var emailService = new GoogleEmailService(model.Email, subject, model.Name, body, true, false);
                emailService.SendMail();
            }
            else
            {
                TempData["Error"] = result.Errors;
                return PartialView("Create", model);
            }

            return Json(new { Success = true });
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Update(string userId, int companyId)
        {
            var user = usersRepository.GetById(userId);
            var company = companyRepository.GetCompany(companyId);
            var employee = new EmployeeViewModel
            {
                Id = userId,
                CompanyId = companyId,
                CompanyName = company?.Name,
                FilePath = usersRepository.GetUserFilePath(userId, FileType.Image),
                Email = user.Email,
                Name = user.UserName,
                Title = user.Title
            };
            ViewBag.IsUpdate = true;
            return PartialView("Create", employee);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Update(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Create", model);
            }

            var user = usersRepository.GetById(model.Id);
            if (user != null)
            {
                user.UserName = model.Name;
                user.Email = model.Email;
                user.Title = model.Title;
                var directoryPath = Server.MapPath(DBO.Common.Constants.UserImagePath);
                await usersRepository.UpdateAsync(user, model.FilePath, directoryPath);
            }

            return Json(new { Success = true });
        }

    }
}