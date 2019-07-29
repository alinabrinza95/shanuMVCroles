using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using shanuMVCUserRoles.Models;

namespace shanuMVCUserRoles.Controllers
{
    public class HolidayViewModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.FirstNameSortParm = string.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "firstname_asc";
            ViewBag.EmailSortParm = string.IsNullOrEmpty(sortOrder) ? "email_desc" : "email_asc";
            ViewBag.TeamLeaderSortParm = string.IsNullOrEmpty(sortOrder) ? "tlname_desc" : "tlname_asc";
            ViewBag.TeamLeaderEmailsSortParm = string.IsNullOrEmpty(sortOrder) ? "tlemail_desc" : "tlemail_asc";
            ViewBag.FlagSortParm = string.IsNullOrEmpty(sortOrder) ? "true" : "false";
            ViewBag.HolidayTypeSortParm = string.IsNullOrEmpty(sortOrder) ? "hol_asc" : "hol_desc";
            ViewBag.SickLeaveIndexSortParm = string.IsNullOrEmpty(sortOrder) ? "sick_asc" : "sick_desc";
            ViewBag.DaysOffSortParm = string.IsNullOrEmpty(sortOrder) ? "daysoff_desc" : "daysoff_asc";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.EndDateSortParm = sortOrder == "EndDate" ? "enddate_desc" : "EndDate";

            
            var holidayRequests = from s in db.AspNetHolidays select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                holidayRequests = holidayRequests.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString));
            }
            
            switch (sortOrder)
            {
                case "name_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.LastName);
                    break;
                case "firstname_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.FirstName);
                    break;
                case "firstname_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.FirstName);
                    break;
                case "email_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.Email);
                    break;
                case "email_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.Email);
                    break;
                case "tlname_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.TeamLeaderName);
                    break;
                case "tlname_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.TeamLeaderName);
                    break;
                case "tlemail_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.TLEmail);
                    break;
                case "tlemail_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.TLEmail);
                    break;
                case "hol_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.HolidayType);
                    break;
                case "hol_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.HolidayType);
                    break;
                case "sick_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.HolidayType);
                    break;
                case "sick_asc":
                    holidayRequests = holidayRequests.OrderBy(s => s.HolidayType);
                    break;
                case "true":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.Flag);
                    break;
                case "false":
                    holidayRequests = holidayRequests.OrderBy(s => s.Flag);
                    break;
                case "Date":
                    holidayRequests = holidayRequests.OrderBy(s => s.StartDate);
                    break;
                case "date_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.StartDate);
                    break;
                case "EndDate":
                    holidayRequests = holidayRequests.OrderBy(s => s.StartDate);
                    break;
                case "enddate_desc":
                    holidayRequests = holidayRequests.OrderByDescending(s => s.StartDate);
                    break;
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

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var holidayViewModel = db.AspNetHolidays.Find(id);

            if (holidayViewModel == null)
            {
                return HttpNotFound();
            }

            return View(holidayViewModel);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Email,TeamLeaderName,StartDate,EndDate,DaysOff,TLEmail,HolidayType,SickLeaveIndex,Flag")] HolidayViewModel holidayViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(holidayViewModel);
            }
            db.AspNetHolidays.Add(holidayViewModel);
            db.SaveChanges();            
            
            return RedirectToAction("SuccessHoliday", "Success");
        }


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

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var holidayViewModel = db.AspNetHolidays.Find(id);

            if (holidayViewModel == null)
            {
                return HttpNotFound();
            }

            return View(holidayViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,TeamLeaderName,StartDate,EndDate,DaysOff,TLEmail,HolidayType,SickLeaveIndex,Flag")] HolidayViewModel holidayViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(holidayViewModel);
            }

            db.Entry(holidayViewModel).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var holidayViewModel = db.AspNetHolidays.Find(id);

            if (holidayViewModel == null)
            {
                return HttpNotFound();
            }

            return View(holidayViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var holidayViewModel = db.AspNetHolidays.Find(id);
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
