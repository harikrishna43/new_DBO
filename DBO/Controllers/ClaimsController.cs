using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DBO.Common;
using DBO.Data;
using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Services;
using DBO.Services.Email;
using Microsoft.AspNet.Identity.Owin;

namespace DBO.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class ClaimsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CompanyRepository _companyRepository = new CompanyRepository();

        // GET: ClaimRequests
        public ActionResult Index()
        {
            var registrations = db.ClaimRequests.Include(r => r.Company);
            return View(registrations.ToList());
        }

        public async Task<ActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClaimRequest request = db.ClaimRequests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }

            var company = _companyRepository.GetCompany(request.CompanyId);
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var newUser = new ApplicationUser { UserName = request.Email, Email = request.Email };
            var password = Guid.NewGuid().ToString().Substring(0, 8) + "!1jK";

            var oldUser = await userManager.FindByNameAsync(newUser.UserName);
            var resultOfDelete = await userManager.DeleteAsync(oldUser);

            if (resultOfDelete.Succeeded)
            {
                var result = await userManager.CreateAsync(newUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser.Id, Constants.CompanyRole);

                    newUser.CompanyId = company.Id;
                    await userManager.UpdateAsync(newUser);

                    var subject = "Company registered";
                    var body = "Your company is registered. Your account password is " + password;

                    var emailService = new GoogleEmailService(
                        company.Email,
                        subject,
                        company.Name,
                        body,
                        true,
                        false
                    );
                    emailService.SendMail();
                }
            }

            request.ClaimStatus = ClaimStatus.Approved;
            request.ApproveTime = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Reject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClaimRequest claimRequest = db.ClaimRequests.Find(id);
            if (claimRequest == null)
            {
                return HttpNotFound();
            }
            claimRequest.ClaimStatus = ClaimStatus.Rejected;
            db.SaveChanges();
            return RedirectToAction("Index");
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
