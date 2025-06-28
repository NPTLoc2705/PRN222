using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvc.app.Models;

namespace mvc.app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            // Get username from session
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

            Console.WriteLine($"Username from session: {username}");
            Console.WriteLine($"ViewData UserMessage: {ViewData["UserMessage"]}");

            return View("Homepage");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}