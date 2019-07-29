using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using shanuMVCUserRoles.Models;

namespace shanuMVCUserRoles.Controllers
{
    public class OOHRequestViewModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.FullNameSortParm = String.IsNullOrEmpty(sortOrder) ? "fullName_desc" : "fullName_asc";
            ViewBag.DaySortParm = sortOrder == "day" ? "day_desc" : "day";
            ViewBag.HoursSortParm = String.IsNullOrEmpty(sortOrder) ? "hours_desc" : "hours_asc";
            ViewBag.TickerNUmberSortParm = String.IsNullOrEmpty(sortOrder) ? "ticketNumber_desc" : "ticketNumber_asc";
            ViewBag.TeamLeaderEmailSortParm = String.IsNullOrEmpty(sortOrder) ? "tlEmail_desc" : "tlEmail_asc";
            ViewBag.FlagSortParm = String.IsNullOrEmpty(sortOrder) ? "false" : "true";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "email_asc";

            var list = from b in db.OOHRequestViewModel
                       select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(b => b.FullName.Contains(searchString));

            }


            switch (sortOrder)
            {
                case "fullName_desc":
                    list = list.OrderByDescending(b => b.FullName);
                    break;
                case "fullName_asc":
                    list = list.OrderBy(b => b.FullName);
                    break;
                case "day_desc":
                    list = list.OrderByDescending(b => b.Day);
                    break;
                case "day":
                    list = list.OrderBy(b => b.Day);
                    break;
                case "hours_desc":
                    list = list.OrderByDescending(b => b.Hours);
                    break;
                case "hours_asc":
                    list = list.OrderBy(b => b.Hours);
                    break;
                case "ticketNumber_desc":
                    list = list.OrderByDescending(b => b.TicketNUmber);
                    break;
                case "ticketNumber_asc":
                    list = list.OrderBy(b => b.TicketNUmber);
                    break;
                case "tlEmail_desc":
                    list = list.OrderByDescending(b => b.TeamLeaderEmail);
                    break;
                case "tlEmail_asc":
                    list = list.OrderBy(b => b.TeamLeaderEmail);
                    break;
                case "false":
                    list = list.OrderByDescending(b => b.Flag);
                    break;
                case "true":
                    list = list.OrderBy(b => b.Flag);
                    break;
                case "emai_dec":
                    list = list.OrderByDescending(b => b.Email);
                    break;
                case "email_asc":
                    list = list.OrderBy(b => b.Email);
                    break;



                default:
                    list = list.OrderBy(b => b.FullName);
                    break;
            }



            return View(list.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OOHRequestViewModel oOHRequestViewModel = db.OOHRequestViewModel.Find(id);
            if (oOHRequestViewModel == null)
            {
                return HttpNotFound();
            }
            return View(oOHRequestViewModel);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FullName,Day,Hours,TicketNUmber,TeamLeaderEmail,Flag,Email")] OOHRequestViewModel oOHRequestViewModel)
        {
            if (ModelState.IsValid)
            {
                db.OOHRequestViewModel.Add(oOHRequestViewModel);
                db.SaveChanges();
                return RedirectToAction("SuccessOOH","Success");
            }

            return View(oOHRequestViewModel);
        }


        public ActionResult InPending()
        {
            var list = from b in db.OOHRequestViewModel
                       join c in db.Users on b.Email equals c.Email
                       where (c.UserName.Equals(User.Identity.Name) && b.Flag.Equals(false))
                       select b;
            return View(list.ToList());
        }

        public ActionResult Approved()
        {
            var list = from b in db.OOHRequestViewModel
                       join c in db.Users on b.Email equals c.Email
                       where (c.UserName.Equals(User.Identity.Name) && b.Flag.Equals(true))
                       select b;
            return View(list.ToList());
        }

        public ActionResult InPendingTeamLeader()
        {
            var list = from b in db.OOHRequestViewModel
                       join c in db.Users on b.TeamLeaderEmail equals c.Email
                       where (c.UserName.Equals(User.Identity.Name) && b.Flag.Equals(false))
                       select b;
            return View(list.ToList());
        }

        public ActionResult ApprovedTeamLeader()
        {
            var list = from b in db.OOHRequestViewModel
                       join c in db.Users on b.TeamLeaderEmail equals c.Email
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
            OOHRequestViewModel oOHRequestViewModel = db.OOHRequestViewModel.Find(id);
            if (oOHRequestViewModel == null)
            {
                return HttpNotFound();
            }
            return View(oOHRequestViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FullName,Day,Hours,TicketNUmber,TeamLeaderEmail,Flag,Email")] OOHRequestViewModel oOHRequestViewModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oOHRequestViewModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(oOHRequestViewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OOHRequestViewModel oOHRequestViewModel = db.OOHRequestViewModel.Find(id);
            if (oOHRequestViewModel == null)
            {
                return HttpNotFound();
            }
            return View(oOHRequestViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OOHRequestViewModel oOHRequestViewModel = db.OOHRequestViewModel.Find(id);
            db.OOHRequestViewModel.Remove(oOHRequestViewModel);
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
