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

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _authService.AuthenticateUser(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // Generate JWT token
            string token = _authService.GenerateToken(user);
            Console.WriteLine("Generated JWT Token: " + token);

            // Store token in session
            HttpContext.Session.SetString("Token", token);

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // recommend using HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });

            // Store user information in session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());

            // Redirect to Home controller Index action
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session data
            Response.Cookies.Delete("AuthToken"); // Clear the auth cookie
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = _authService.GetUserByEmail(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Password = model.Password
            };

            user = _authService.RegisterUser(user);

            Console.WriteLine("User registered successfully: " + user.Id);

            // Redirect to Login page after successful registration
            return RedirectToAction("Login", "Auth");
        }
    }
}