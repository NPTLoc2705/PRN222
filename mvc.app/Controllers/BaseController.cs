using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace mvc.app.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
            {
                ViewData["UserMessage"] = $"Welcome, {username}!";
                ViewData["IsLoggedIn"] = true;
            }
            else
            {
                ViewData["UserMessage"] = "Welcome, Guest!";
                ViewData["IsLoggedIn"] = false;
            }

            base.OnActionExecuting(context);
        }
    }
}
