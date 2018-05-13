using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DBO.Data;
using DBO.Data.Models;

namespace DBO.Controllers
{
    public class CompanySkillsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CompanySkills
        public ActionResult Index()
        {
            var companySkills = db.CompanySkills.Include(c => c.Company).Include(c => c.Skill);
            return View(companySkills.ToList());
        }

        // GET: CompanySkills/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanySkill companySkill = db.CompanySkills.Find(id);
            if (companySkill == null)
            {
                return HttpNotFound();
            }
            return View(companySkill);
        }

        // GET: CompanySkills/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Companies.Take(100), "Id", "Name");
            ViewBag.SkillId = new SelectList(db.Skills, "Id", "Name");
            return View();
        }

        // POST: CompanySkills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CompanyId,SkillId")] CompanySkill companySkill)
        {
            if (ModelState.IsValid)
            {
                db.CompanySkills.Add(companySkill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Companies.Take(100), "Id", "Name", companySkill.CompanyId);
            ViewBag.SkillId = new SelectList(db.Skills, "Id", "Name", companySkill.SkillId);
            return View(companySkill);
        }

        // GET: CompanySkills/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanySkill companySkill = db.CompanySkills.Find(id);
            if (companySkill == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies.Take(100), "Id", "Name", companySkill.CompanyId);
            ViewBag.SkillId = new SelectList(db.Skills, "Id", "Name", companySkill.SkillId);
            return View(companySkill);
        }

        // POST: CompanySkills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyId,SkillId")] CompanySkill companySkill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companySkill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Companies.Take(100), "Id", "Name", companySkill.CompanyId);
            ViewBag.SkillId = new SelectList(db.Skills, "Id", "Name", companySkill.SkillId);
            return View(companySkill);
        }

        // GET: CompanySkills/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanySkill companySkill = db.CompanySkills.Find(id);
            if (companySkill == null)
            {
                return HttpNotFound();
            }
            return View(companySkill);
        }

        // POST: CompanySkills/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompanySkill companySkill = db.CompanySkills.Find(id);
            db.CompanySkills.Remove(companySkill);
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
