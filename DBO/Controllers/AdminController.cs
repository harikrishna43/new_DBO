using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using DBO.App_Start;
using DBO.Common;
using DBO.Data;
using DBO.Data.Migrations;
using WebGrease;
using Microsoft.AspNet.Identity;
using DBO.Extensions;
using Microsoft.AspNet.Identity.Owin;

namespace DBO.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole)]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Companies");
        }

        [ActionName("seed")]
        public ActionResult Seed()
        {
            var conf = new Configuration();
            conf.InitUsers(ApplicationDbContext.Create(), HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());
            return new HttpStatusCodeResult(200);
        }

        [AllowAnonymous]
        [ActionName("resources")]
        public ActionResult Resources()
        {
            var resourceConfig = new ResourceConfig();
            resourceConfig.PopulateResources().Wait();
            return new HttpStatusCodeResult(200);
        }

        public ActionResult UserStat()
        {
            var userId = User.GetClaimValue(Common.Constants.UserIdClaim);
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.Identity.GetUserId();
            }

            string res = $"UserId: {userId}<br/>";

            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims;
            foreach (Claim claim in claims)
            {
                res += $"{claim.Subject.Name} {claim.Type} {claim.Value}<br/>";
            }

            return Content(res);
        }

    }
}