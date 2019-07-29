using shanuMVCUserRoles.Models;
using System.Web.Mvc;
using System.Linq;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using shanuMVCUserRoles.Resources;

namespace shanuMVCUserRoles.Controllers
{
    public class HomeController : Controller
	{
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = ControllerResources.ApplicationDescriptionPage;

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = ControllerResources.ContactPage;

			return View();
		}

        public string GetUserRole()
        {
            var userRole = string.Empty;

            if (!User.Identity.IsAuthenticated)
            {
                return userRole;
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            userRole = userManager.GetRoles(User.Identity.GetUserId())[0];

            return userRole;
        }

        public ActionResult TeamLeaderStatistics()
        {
            var teamLeaderEmployees = Enumerable.Empty<ProfileViewModel>().AsQueryable();

            int holidayRequestsInPending = 0;
            int holidayRequestsApproved = 0;
            int holidayRequests = 0;

            int oohRequestsInPending = 0;
            int oohRequestsApproved = 0;
            int oohRequests = 0;

            if (User.Identity.IsAuthenticated)
            {
                if (GetUserRole() == ControllerResources.TeamLeader)
                {  
                    var user = User.Identity.Name;

                    string team = (from b in db.ProfileViewModel
                                   where b.UserName == user
                                   select b.Team).Single();
                    string email = (from b in db.ProfileViewModel
                                    where b.UserName == user
                                    select b.Email).Single();
                    
                    teamLeaderEmployees = from b in db.ProfileViewModel
                                              join c in db.Users on b.Email equals c.Email
                                              where b.Team == team
                                              select b;
                    holidayRequestsInPending = (from b in db.AspNetHolidays
                                                where (b.StartDate.Month.Equals(DateTime.Now.Month) && b.TLEmail.Equals(email)
                                                && b.Flag.Equals(false))
                                                select b).Count();
                    holidayRequestsApproved = (from b in db.AspNetHolidays
                                                where (b.StartDate.Month.Equals(DateTime.Now.Month) && b.TLEmail.Equals(email)
                                                && b.Flag.Equals(true))
                                                select b).Count();
                    holidayRequests = (from b in db.AspNetHolidays
                                               where (b.StartDate.Month.Equals(DateTime.Now.Month) && b.TLEmail.Equals(email))
                                               select b).Count();
                    ViewBag.holidayRequestsInPending = holidayRequestsInPending;
                    ViewBag.holidayRequestsApproved = holidayRequestsApproved;
                    ViewBag.holidayRequests = holidayRequests;

                    oohRequestsInPending = (from b in db.OOHRequestViewModel
                                            where (b.Day.Month.Equals(DateTime.Now.Month) && b.TeamLeaderEmail.Equals(email)
                                            && b.Flag.Equals(false))
                                            select b).Count();
                    oohRequestsApproved = (from b in db.OOHRequestViewModel
                                            where (b.Day.Month.Equals(DateTime.Now.Month) && b.TeamLeaderEmail.Equals(email)
                                            && b.Flag.Equals(true))
                                            select b).Count();
                    oohRequests = (from b in db.OOHRequestViewModel
                                           where (b.Day.Month.Equals(DateTime.Now.Month) && b.TeamLeaderEmail.Equals(email))
                                           select b).Count();
                    ViewBag.oohRequestsInPending = oohRequestsInPending;
                    ViewBag.oohRequestsApproved = oohRequestsApproved;
                    ViewBag.oohRequests = oohRequests;
                }
                          
            }
            return View(teamLeaderEmployees.ToList());

        }

        public ActionResult ManagerStatistics()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (GetUserRole() == "Manager")
                {
                    
                }
            }
            return View();
        }     

      
        public ActionResult GenericDashboard(string teamName)
        {
            var teamEmployees = Enumerable.Empty<ProfileViewModel>().AsQueryable();

            int holidayRequestsInPending = 0;
            int holidayRequestsApproved = 0;
            int holidayRequests = 0;

            int oohRequestsInPending = 0;
            int oohRequestsApproved = 0;
            int oohRequests = 0;

            teamEmployees = from b in db.ProfileViewModel
                                          where b.Team.Equals(teamName)
                                          select b;


            holidayRequestsInPending = (from b in db.AspNetHolidays
                                        join c in db.ProfileViewModel on b.Email equals c.Email
                                        where (c.Team.Equals(teamName) && b.StartDate.Month.Equals(DateTime.Now.Month) && b.Flag.Equals(false))
                                        select b).Count();
            holidayRequestsApproved = (from b in db.AspNetHolidays
                                       join c in db.ProfileViewModel on b.Email equals c.Email
                                       where (c.Team.Equals(teamName) && b.StartDate.Month.Equals(DateTime.Now.Month) && b.Flag.Equals(true))
                                       select b).Count();
            holidayRequests = (from b in db.AspNetHolidays
                               join c in db.ProfileViewModel on b.Email equals c.Email
                               where (c.Team.Equals(teamName) && b.StartDate.Month.Equals(DateTime.Now.Month))
                               select b).Count();
            ViewBag.holidayRequestsInPending = holidayRequestsInPending;
            ViewBag.holidayRequestsApproved = holidayRequestsApproved;
            ViewBag.holidayRequests = holidayRequests;

            oohRequestsInPending = (from b in db.OOHRequestViewModel
                                    join c in db.ProfileViewModel on b.Email equals c.Email
                                    where (c.Team.Equals(teamName) && b.Day.Month.Equals(DateTime.Now.Month) && b.Flag.Equals(false))
                                    select b).Count();
            oohRequestsApproved = (from b in db.OOHRequestViewModel
                                   join c in db.ProfileViewModel on b.Email equals c.Email
                                   where (c.Team.Equals(teamName) && b.Day.Month.Equals(DateTime.Now.Month) && b.Flag.Equals(true))
                                   select b).Count();
            oohRequests = (from b in db.OOHRequestViewModel
                           join c in db.ProfileViewModel on b.Email equals c.Email
                           where (c.Team.Equals(teamName) && b.Day.Month.Equals(DateTime.Now.Month))
                           select b).Count();
            ViewBag.oohRequestsInPending = oohRequestsInPending;
            ViewBag.oohRequestsApproved = oohRequestsApproved;
            ViewBag.oohRequests = oohRequests;


            return View(teamEmployees.ToList());
        }


        public ActionResult Applicationsupport()
        {
            return GenericDashboard(ControllerResources.ApplicationSupport);
        }

        public ActionResult SoftwareDevelopment()
        {
            return GenericDashboard(ControllerResources.SoftwareDevelopment);
        }

        public ActionResult BusinessIntelligence()
        {
            return GenericDashboard(ControllerResources.BusinessIntelligence);
        }

        public ActionResult SQLDBA()
        {
            return GenericDashboard(ControllerResources.SQLDBA);
        }

        public ActionResult OraDBA()
        {
            return GenericDashboard(ControllerResources.OraDBA);
        }

        public ActionResult Middleware()
        {
            return GenericDashboard(ControllerResources.Middleware);
        }

        public ActionResult UNIX()
        {
            return GenericDashboard(ControllerResources.UNIX);
        }


    }
}