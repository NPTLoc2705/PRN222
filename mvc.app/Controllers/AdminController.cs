using Microsoft.AspNetCore.Mvc;

namespace mvc.app.Controllers
{
    public class AdminController : Controller
    {

        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("Role");            // This action could be used to display admin dashboard information
          /*  if(role != "Admin")
            {
                // If the user is not an admin, redirect to the home page or an error page
                return RedirectToAction("Index", "Home");
            }*/
            return View("AdminPage");
        }


    }
}
