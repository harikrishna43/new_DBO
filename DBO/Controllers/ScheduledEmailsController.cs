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
    public class ScheduledEmailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ScheduledEmails
        public ActionResult Index()
        {
            var scheduledEmails = db.ScheduledEmails.Include(s => s.Company).Where(e => e.Status != EmailStatus.Sent);
            return View(scheduledEmails.ToList());
        }

        // GET: ScheduledEmails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledEmail scheduledEmail = db.ScheduledEmails.Find(id);
            if (scheduledEmail == null)
            {
                return HttpNotFound();
            }
            return View(scheduledEmail);
        }

        // GET: ScheduledEmails/Create
        public ActionResult Create()
        {
            //ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name");
            return View();
        }

        // POST: ScheduledEmails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CompanyId,Subject,Email,Body,Status,CreatedAt,UpdatedAt")] ScheduledEmail scheduledEmail)
        {
            if (ModelState.IsValid)
            {
                db.ScheduledEmails.Add(scheduledEmail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", scheduledEmail.CompanyId);
            return View(scheduledEmail);
        }

        // GET: ScheduledEmails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledEmail scheduledEmail = db.ScheduledEmails.Find(id);
            if (scheduledEmail == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", scheduledEmail.CompanyId);
            return View(scheduledEmail);
        }

        // POST: ScheduledEmails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyId,Subject,Email,Body,Status,CreatedAt,UpdatedAt")] ScheduledEmail scheduledEmail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduledEmail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", scheduledEmail.CompanyId);
            return View(scheduledEmail);
        }

        // GET: ScheduledEmails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledEmail scheduledEmail = db.ScheduledEmails.Find(id);
            if (scheduledEmail == null)
            {
                return HttpNotFound();
            }
            return View(scheduledEmail);
        }

        // POST: ScheduledEmails/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ScheduledEmail scheduledEmail = db.ScheduledEmails.Find(id);
            db.ScheduledEmails.Remove(scheduledEmail);
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
