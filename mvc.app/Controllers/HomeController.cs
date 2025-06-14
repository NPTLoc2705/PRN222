using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvc.app.Models;

namespace mvc.app.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    //[Authorize(Roles = "Member, Staff, Consultant, Manager, Admin")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        var username = User.Identity?.Name;
        Console.WriteLine($"Username: {username}");
        ViewData["UserMessage"] = $"Welcome, {username}.";
        return View("Homepage");
    }

    //[Authorize(Roles = "Member, Staff, Consultant, Manager, Admin")]
    //[HttpGet("Home/Homepage")]
    //public IActionResult Homepage()
    //{
    //    var username = User.Identity?.Name ?? "Guest";
    //    ViewData["UserMessage"] = $"Welcome, {username}.";
    //    return View("Homepage");
    //}

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
