using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DBO.Common;
using DBO.Data;
using DBO.Data.Models;

namespace DBO.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class RegistrationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RegistrationCodes
        public ActionResult Index()
        {
            var registrationCodes = db.RegistrationCodes.Include(r => r.Company).ToList();
            var invited = registrationCodes.Where(r => r.Generated != null && r.Registered == null);
            var invitedRegistered = registrationCodes.Where(r => r.Generated != null && r.Registered != null);
            var companies = registrationCodes.Where(r => r.Generated == null && r.IsCompany);
            var employees = registrationCodes.Where(r => r.Generated == null && !r.IsCompany);

            var model = new AdminRegistrationViewModel
            {
                Invited = invited,
                InvitedRegistered = invitedRegistered,
                Companies = companies,
                Employees = employees
            };

            return View(model);
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

    public class AdminRegistrationViewModel
    {
        public IEnumerable<RegistrationCode> Invited { get; set; }
        public IEnumerable<RegistrationCode> InvitedRegistered { get; set; }
        public IEnumerable<RegistrationCode> Companies { get; set; }
        public IEnumerable<RegistrationCode> Employees { get; set; }
    }
}
