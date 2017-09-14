﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using shanuMVCUserRoles.Models;
using System;
namespace shanuMVCUserRoles.Controllers
{
    public class HolidayViewModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HolidayViewModels
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "firstname_asc";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "email_asc";
            ViewBag.TeamLeaderSortParm = String.IsNullOrEmpty(sortOrder) ? "tlname_desc" : "tlname_asc";
            ViewBag.TeamLeaderEmailsSortParm = String.IsNullOrEmpty(sortOrder) ? "tlemail_desc" : "tlemail_asc";
            ViewBag.FlagSortParm = String.IsNullOrEmpty(sortOrder) ? "true" : "false";
            ViewBag.HolidayTypeSortParm = String.IsNullOrEmpty(sortOrder) ? "hol_asc" : "hol_desc";
            ViewBag.SickLeaveIndexSortParm = String.IsNullOrEmpty(sortOrder) ? "sick_asc" : "sick_desc";
            ViewBag.DaysOffSortParm = String.IsNullOrEmpty(sortOrder) ? "daysoff_desc" : "daysoff_asc";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.EndDateSortParm = sortOrder == "EndDate" ? "enddate_desc" : "EndDate";

            
            var holidayRequests = from s in db.AspNetHolidays
                           select s;


            if (!String.IsNullOrEmpty(searchString))
            {
                holidayRequests = holidayRequests.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString));
            }

            
            switch (sortOrder)
            {
                //order by last name
                case "name_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.LastName);
                    break;

                    //order by first name
                case "firstname_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.FirstName);
                    break;
                case "firstname_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.FirstName);
                    break;

                //order by email
                case "email_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.Email);
                    break;
                case "email_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.Email);
                    break;

                //order by tl name
                case "tlname_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.TeamLeaderName);
                    break;
                case "tlname_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.TeamLeaderName);
                    break;

                //order by tlemail
                case "tlemail_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.TLEmail);
                    break;
                case "tlemail_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.TLEmail);
                    break;

                //order by holiday
                case "hol_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.HolidayType);
                    break;
                case "hol_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.HolidayType);
                    break;

                //order by sick leave index
                case "sick_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.HolidayType);
                    break;
                case "sick_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.HolidayType);
                    break;

                //order by flag
                case "true":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.Flag);
                    break;
                case "false":
                    holidayRequests = holidayRequests.OrderBy(s => s.Flag);
                    break;

                //order by start date
                case "Date":
                    holidayRequests = holidayRequests.OrderBy(s => s.StartDate);
                    break;
                case "date_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.StartDate);
                    break;

                //order by end date
                case "EndDate":
                    holidayRequests = holidayRequests.OrderBy(s => s.StartDate);
                    break;
                case "enddate_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.StartDate);
                    break;

                //order by end date
                case "daysoff_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.DaysOff);
                    break;
                case "daysoff_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.DaysOff);
                    break;

                default:
                    holidayRequests = holidayRequests.OrderBy(s => s.LastName);
                    break;
            }

            return View(holidayRequests.ToList());
        }

        // GET: HolidayViewModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HolidayViewModel holidayViewModel = db.AspNetHolidays.Find(id);
            if (holidayViewModel == null)
            {
                return HttpNotFound();
            }
            return View(holidayViewModel);
        }

        // GET: HolidayViewModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HolidayViewModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Email,TeamLeaderName,StartDate,EndDate,DaysOff,TLEmail,HolidayType,SickLeaveIndex,Flag")] HolidayViewModel holidayViewModel)
        {
            

            if (ModelState.IsValid)
            {
                
                db.AspNetHolidays.Add(holidayViewModel);
                db.SaveChanges();               
                return RedirectToAction("SuccessHoliday", "Success");
            }

            return View(holidayViewModel);
        }


        //employee inpending and approoved
        public ActionResult InPending()
        {
            var list = from b in db.AspNetHolidays
                       join c in db.Users on b.Email equals c.Email
                       where (c.UserName.Equals(User.Identity.Name) && b.Flag.Equals(false))
                       select b;
            return View(list.ToList());
        }      


        public ActionResult Approved()
        {
            var list = from b in db.AspNetHolidays
                       join c in db.Users on b.Email equals c.Email
                       where (c.UserName.Equals(User.Identity.Name) && b.Flag.Equals(true))
                       select b;
            return View(list.ToList());
        }

        //teamleader inpending and aproved
        public ActionResult InPendingTeamLeader()
        {
            var list = from b in db.AspNetHolidays
                       join c in db.Users on b.TLEmail equals c.Email
                       where (c.UserName.Equals(User.Identity.Name) && b.Flag.Equals(false))
                       select b;
            return View(list.ToList());
        }

        public ActionResult ApprovedTeamLeader()
        {
            var list = from b in db.AspNetHolidays
                       join c in db.Users on b.TLEmail equals c.Email
                       where (c.UserName.Equals(User.Identity.Name) && b.Flag.Equals(true))
                       select b;
            return View(list.ToList());
        }


        // GET: HolidayViewModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HolidayViewModel holidayViewModel = db.AspNetHolidays.Find(id);
            if (holidayViewModel == null)
            {
                return HttpNotFound();
            }
            return View(holidayViewModel);
        }

        // POST: HolidayViewModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,TeamLeaderName,StartDate,EndDate,DaysOff,TLEmail,HolidayType,SickLeaveIndex,Flag")] HolidayViewModel holidayViewModel)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(holidayViewModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(holidayViewModel);
        }

        

        // GET: HolidayViewModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HolidayViewModel holidayViewModel = db.AspNetHolidays.Find(id);
            if (holidayViewModel == null)
            {
                return HttpNotFound();
            }
            return View(holidayViewModel);
        }

        // POST: HolidayViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HolidayViewModel holidayViewModel = db.AspNetHolidays.Find(id);
            db.AspNetHolidays.Remove(holidayViewModel);
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
