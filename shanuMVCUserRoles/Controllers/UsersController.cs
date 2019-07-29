using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using shanuMVCUserRoles.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using shanuMVCUserRoles.Resources;

namespace shanuMVCUserRoles.Controllers
{
    [Authorize]
	public class UsersController : Controller
    {
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


        public ActionResult Index()
		{
			if (User.Identity.IsAuthenticated)
			{
				var user = User.Identity;

				ViewBag.Name = user.Name;
				ViewBag.displayMenu = ControllerResources.No;
                ViewBag.displayMenu = GetUserRole();

                return View();
			}

            ViewBag.Name = ControllerResources.NotLoggedIn;

            return View();
		}
	}
}