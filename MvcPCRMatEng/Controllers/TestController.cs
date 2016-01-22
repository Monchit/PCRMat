using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPCRMatEng.Models;

namespace MvcPCRMatEng.Controllers
{
    public class TestController : Controller
    {
        private PCRMatEngEntities db = new PCRMatEngEntities();

        //
        // GET: /Test/

        public ActionResult Index()
        {
            var td_pcr = db.TD_PCR.Include(t => t.TM_GroupCode).Include(t => t.TM_Purpose);
            return View(td_pcr.ToList());
        }

        //
        // GET: /Test/Details/5

        public ActionResult Details(string groupcode = null, string year = null, int runno = 0)
        {
            TD_PCR td_pcr = db.TD_PCR.Find(groupcode, year, runno);
            if (td_pcr == null)
            {
                return HttpNotFound();
            }
            return View(td_pcr);
        }

        //
        // GET: /Test/Create

        public ActionResult Create()
        {
            ViewBag.group_code = new SelectList(db.TM_GroupCode, "group_code", "group_code");
            ViewBag.purpose_id = new SelectList(db.TM_Purpose, "purpose_id", "purpose_name");
            return View();
        }

        //
        // POST: /Test/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TD_PCR td_pcr)
        {
            if (ModelState.IsValid)
            {
                db.TD_PCR.Add(td_pcr);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.group_code = new SelectList(db.TM_GroupCode, "group_code", "group_code", td_pcr.group_code);
            ViewBag.purpose_id = new SelectList(db.TM_Purpose, "purpose_id", "purpose_name", td_pcr.purpose_id);
            return View(td_pcr);
        }

        //
        // GET: /Test/Edit/5

        public ActionResult Edit(string groupcode = null, string year = null, int runno = 0)
        {
            TD_PCR td_pcr = db.TD_PCR.Find(groupcode, year, runno);
            if (td_pcr == null)
            {
                return HttpNotFound();
            }
            ViewBag.group_code = new SelectList(db.TM_GroupCode, "group_code", "group_code", td_pcr.group_code);
            ViewBag.purpose_id = new SelectList(db.TM_Purpose, "purpose_id", "purpose_name", td_pcr.purpose_id);
            return View(td_pcr);
        }

        //
        // POST: /Test/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TD_PCR td_pcr)
        {
            if (ModelState.IsValid)
            {
                db.Entry(td_pcr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.group_code = new SelectList(db.TM_GroupCode, "group_code", "update_by", td_pcr.group_code);
            ViewBag.purpose_id = new SelectList(db.TM_Purpose, "purpose_id", "purpose_name", td_pcr.purpose_id);
            return View(td_pcr);
        }

        //
        // GET: /Test/Delete/5

        public ActionResult Delete(string groupcode = null, string year = null, int runno = 0)
        {
            TD_PCR td_pcr = db.TD_PCR.Find(groupcode, year, runno);
            if (td_pcr == null)
            {
                return HttpNotFound();
            }
            return View(td_pcr);
        }

        //
        // POST: /Test/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string groupcode, string year, int runno)
        {
            TD_PCR td_pcr = db.TD_PCR.Find(groupcode, year, runno);
            db.TD_PCR.Remove(td_pcr);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}