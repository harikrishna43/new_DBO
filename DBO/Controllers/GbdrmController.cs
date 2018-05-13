using System.Web.Mvc;
using DBO.Common;
using DBO.Services.Email;

namespace DBO.Controllers
{
    public class GbdrmController : Controller
    {
        [Authorize(Roles = Constants.AdminRole)]
        public ActionResult TestEmail()
        {
            IEmailService service = new GoogleEmailService("lajdak.ua@gmail.com", "test", "test", "test", false, false);
#if DEBUG
            service = new SendGridEmailService("lajdak.ua@gmail.com", "test", "test", "test", false, false);
#endif
            
            return Content(service.SendMail());
        }
    }
}