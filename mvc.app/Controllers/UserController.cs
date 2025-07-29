using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvc.dataaccess.ViewModels.User;
using mvc.services.Implements;
using mvc.services.Interfaces;

namespace mvc.app.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }



        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // Helper method to check if user is admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Access denied. Admin only.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var users = await _userService.GetAllUsers();
                ViewBag.CurrentUserId = HttpContext.Session.GetString("UserId");
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading users: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Profile(Guid id)
        {
            if (!IsUserLoggedIn())
            {
                TempData["Error"] = "Please login to view profiles.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var user = _userService.GetUserProfile(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Index");
                }
                return View("UserProfile", user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading profile: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet("/User/UserProfile")]
        public IActionResult MyProfile()
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    TempData["Error"] = "Please login to view your profile.";
                    return RedirectToAction("Login", "Auth");
                }

                var userId = Guid.Parse(userIdString);
                var user = _userService.GetUserProfile(userId);

                if (user == null)
                {
                    TempData["Error"] = "Profile not found";
                    return RedirectToAction("Index", "Home");
                }

                return View("UserProfile", user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading profile: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "You are not authorized.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var result = _userService.DeleteUser(id);
                if (result)
                {
                    TempData["Success"] = "User deleted successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to delete user";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting user: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ban(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "You are not authorized.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var result = _userService.BanUser(id);
                if (result)
                {
                    TempData["Success"] = "User banned successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to ban user";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error banning user: " + ex.Message;
            }
            return RedirectToAction("DashBoard","Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UnBan(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "You are not authorized.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var result = _userService.UnBanUser(id);
                if (result)
                {
                    TempData["Success"] = "User unbanned successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to unban user";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error unbanning user: " + ex.Message;
            }
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(UpdateUserViewModel model)
        {
            var currentUserId = HttpContext.Session.GetString("UserId");
            if (currentUserId == null || model.Id.ToString() != currentUserId)
                return Unauthorized();

            try
            {
                _userService.UpdateUserProfile(model);
                TempData["Success"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to update profile: " + ex.Message;
            }

            return RedirectToAction("MyProfile");
        }
    }
}