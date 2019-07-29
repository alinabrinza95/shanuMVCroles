using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using shanuMVCUserRoles.Models;
using System;

namespace shanuMVCUserRoles.Controllers
{
    public class ProfileViewModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View(db.ProfileViewModel.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfileViewModel profileViewModel = db.ProfileViewModel.Find(id);
            if (profileViewModel == null)
            {
                return HttpNotFound();
            }
            return View(profileViewModel);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Username,FirstName,LastName,Emai,Mark,CNP,Location,Team,TeamLeaderEmail")] ProfileViewModel profileViewModel)
        {
            if (ModelState.IsValid)
            {
                profileViewModel.UserName= User.Identity.Name;
                profileViewModel.FirstName = Request.Form["FirstName"];
                profileViewModel.LastName = Request.Form["LastName"];
                profileViewModel.Email = Request.Form["Email"];
                profileViewModel.Mark = Convert.ToInt32(Request.Form["Mark"]);
                profileViewModel.CNP = Request.Form["CNP"];
                profileViewModel.Location = Request.Form["Location"];
                profileViewModel.Team = Request.Form["Team"];
                profileViewModel.TeamLeaderEmail = Request.Form["TeamLeaderEmail"];

                db.ProfileViewModel.Add(profileViewModel);
                db.SaveChanges();
                return RedirectToAction("SuccessRegistration", "Success");
            }

            return View(profileViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfileViewModel profileViewModel = db.ProfileViewModel.Find(id);
            if (profileViewModel == null)
            {
                return HttpNotFound();
            }
            return View(profileViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,FirstName,LastName,Email,Mark,CNP,Location,Team,TeamLeaderEmail")] ProfileViewModel profileViewModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profileViewModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profileViewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfileViewModel profileViewModel = db.ProfileViewModel.Find(id);
            if (profileViewModel == null)
            {
                return HttpNotFound();
            }
            return View(profileViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProfileViewModel profileViewModel = db.ProfileViewModel.Find(id);
            db.ProfileViewModel.Remove(profileViewModel);
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
