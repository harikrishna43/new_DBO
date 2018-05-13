using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DBO.Controllers
{
    public class StatController : Controller
    {
        // GET: Stat
        public async Task<ActionResult> Index()
        {
            var identity = User.Identity;
            var userId = identity.GetUserId();
            var claims = await HttpContext
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>()
                .GetClaimsAsync(userId);

            string res = $"User name: {identity.Name}\r\n";
            foreach (var claim in claims)
            {
                res += $"Claim: {claim.Type} - {claim.Value} \r\n";
            }

            return Content(res);
        }
    }
}