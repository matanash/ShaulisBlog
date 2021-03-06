﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShaulisBlog.Models;

namespace ShaulisBlog.Controllers
{
    public class FansClubController : Controller
    {
        private ShaulisBlogContext db = new ShaulisBlogContext();

      
        public ActionResult Index(string firstName, string lastName, string gender)
        {
            var GenderLst = new List<Gender>();
            var GenderQry = from f in db.Fans
                            orderby f._gender
                            select f._gender;

            GenderLst.AddRange(GenderQry.Distinct());
            ViewBag.gender = new SelectList(GenderLst);
            var fans = from fan in db.Fans
                       select fan;

            if (!String.IsNullOrEmpty(firstName))
            {
                fans = fans.Where(s => s._firstName.Contains(firstName));
            }
            if (!String.IsNullOrEmpty(lastName))
            {
                fans = fans.Where(s => s._lastName.Contains(lastName));
            }
            if (!string.IsNullOrEmpty(gender))
            {
                fans = fans.Where(x => x._gender.ToString() == gender);
            }
            return View(fans);
        }
        public ActionResult GroupFansByGender()
        {
            IQueryable<FanToGenderGroup> data = from fan in db.Fans
                                              group fan by fan._gender into Gender
                                              select new FanToGenderGroup()
                                              {
                                                  gender = Gender.Key,
                                                  fansCount = Gender.Count()
                                              };
            return View(data);           
        }
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fan fan = db.Fans.Find(id);
            if (fan == null)
            {
                return HttpNotFound();
            }
            return View(fan);
        }

        // GET: Fans/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Fans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,_firstName,_lastName,_gender,_birthDate,_seniority,_address")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                db.Fans.Add(fan);
                db.SaveChanges();
                return RedirectToAction("Index", "FansClub", null);
            }

            return View(fan);
        }

        // GET: Fans/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fan fan = db.Fans.Find(id);
            if (fan == null)
            {
                return HttpNotFound();
            }
            return View(fan);
        }

        // POST: Fans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "ID,_firstName,_lastName,_gender,_birthDate,_seniority,_address")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fan);
        }

        // GET: Fans/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fan fan = db.Fans.Find(id);
            if (fan == null)
            {
                return HttpNotFound();
            }
            return View(fan);
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            Fan fan = db.Fans.Find(id);
            db.Fans.Remove(fan);
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