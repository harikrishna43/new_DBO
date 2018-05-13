using System.Linq;
using System.Web.Mvc;
using DBO.Common;
using DBO.Data;

namespace DBO.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class AdClicksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AdClicks
        public ActionResult Index()
        {
            return View(db.AdClicks.ToList());
        }
    }
}