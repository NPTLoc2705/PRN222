using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mvc.dataaccess.Entities;
using mvc.services.Interfaces;
using mvc.dataaccess.ViewModels;

namespace mvc.app.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _authService.AuthenticateUser(model.Email, model.Password);

                if (user != null)
                {
                    // Generate JWT token
                    string token = _authService.GenerateToken(user);
                    
                    // Store token in session
                    HttpContext.Session.SetString("JWTToken", token);
                    
                    // Also store user information for backward compatibility
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Username", user.FullName);
                    HttpContext.Session.SetString("UserRole", user.Role.ToString());
                    
                    return RedirectToAction("Homepage", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session data
            return RedirectToAction("Login");
        }
    }
}
