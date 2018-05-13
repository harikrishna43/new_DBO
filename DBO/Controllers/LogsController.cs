using System;
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
    public class LogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Logs
        public ActionResult Index()
        {
            return View(db.Logs.ToList());
        }

        public ActionResult Clear()
        {
            var logItems = db.Logs.Where(l => DbFunctions.TruncateTime(l.Time) < DbFunctions.TruncateTime(DateTime.Now)).ToList();
            db.Logs.RemoveRange(logItems);
            db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Logs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogItem logItem = db.Logs.Find(id);
            if (logItem == null)
            {
                return HttpNotFound();
            }
            return View(logItem);
        }

        // GET: Logs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Logs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Value,Time")] LogItem logItem)
        {
            if (ModelState.IsValid)
            {
                db.Logs.Add(logItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(logItem);
        }

        // GET: Logs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogItem logItem = db.Logs.Find(id);
            if (logItem == null)
            {
                return HttpNotFound();
            }
            return View(logItem);
        }

        // POST: Logs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Value,Time")] LogItem logItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(logItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(logItem);
        }

        // GET: Logs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogItem logItem = db.Logs.Find(id);
            if (logItem == null)
            {
                return HttpNotFound();
            }
            return View(logItem);
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LogItem logItem = db.Logs.Find(id);
            db.Logs.Remove(logItem);
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
