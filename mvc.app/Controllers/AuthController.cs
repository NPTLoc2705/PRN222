using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mvc.dataaccess.Entities;
using mvc.services.Interfaces;
using mvc.dataaccess.ViewModels;

namespace mvc.app.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService; // Inject your account service

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
            
            if (ModelState.IsValid)
            {
                var user = _authService.AuthenticateUser(model.Email, model.Password);
                
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                if (user != null)
                {
                    // Store user information in session
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Username", user.FullName);
                    Console.WriteLine(user.Role);

                    return RedirectToAction("", ""); // Redirect to home page
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
        
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            
            if (ModelState.IsValid)
            {
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
                
                return RedirectToAction("", ""); // Redirect to home page
            }

            return View(model);
        }
    }

}
