using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;
using mvc.services.Interfaces;

namespace mvc.app.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IBookingService _bookingService;
        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;
        private readonly ICategoryService _categoryService;
        private readonly IProgressService _progressService;
        private readonly IUserService _userService;
        private readonly IBlogService _blogService;

        public AdminController(
            IBookingService bookingService,
            IUserService userService,
            ICourseService courseService,
            ILessonService lessonService,
            IProgressService progressService,
            ICategoryService categoryService,
            IBlogService blogService)
        {
            _bookingService = bookingService;
            _userService = userService;
            _courseService = courseService;
            _lessonService = lessonService;
            _progressService = progressService;
            _categoryService = categoryService;
            _blogService = blogService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null || role != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var bookings = await _bookingService.GetAllBookingsWithNamesAsync();
                var users = await _userService.GetAllUsers();
                var categories = await _categoryService.GetAllCategoriesAsync();
                var blogs = _blogService.GetAll(); // Using service layer for blogs
                ViewBag.Users = users;
                ViewBag.CurrentUserId = userId;
                ViewBag.Categories = categories;
                ViewBag.Blogs = blogs;
                return View("AdminDashboard", bookings);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading dashboard data: " + ex.Message;
                return View("AdminDashboard", new List<BookingViewModel>());
            }
        }

        // Blog Management Actions
        public IActionResult BlogIndex()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            var blogs = _blogService.GetAll();
            return View("~/Views/Admin/BlogIndex.cshtml", blogs);
        }

        public IActionResult BlogCreate()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            return View("~/Views/Admin/BlogCreate.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BlogCreate([Bind("Id,blog_content,ImageData,title")] Blog blog)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userIdString))
                {
                    blog.UserId = Guid.Parse(userIdString);
                    _blogService.Add(blog);
                    TempData["Success"] = "Blog created successfully.";
                    return RedirectToAction(nameof(Dashboard));
                }
                return Unauthorized();
            }
            return View("~/Views/Admin/BlogCreate.cshtml", blog);
        }

        public IActionResult BlogEdit(Guid id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var blog = _blogService.GetById(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/BlogEdit.cshtml", blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BlogEdit(Guid id, [Bind("Id,blog_content,ImageData,title")] Blog blog)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _blogService.Update(blog);
                    TempData["Success"] = "Blog updated successfully.";
                }
                catch (Exception)
                {
                    if (_blogService.GetById(blog.Id) == null)
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Dashboard));
            }
            return View("~/Views/Admin/BlogEdit.cshtml", blog);
        }

        public IActionResult BlogDelete(Guid? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var blog = _blogService.GetById(id.Value);
            if (blog == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/BlogDelete.cshtml", blog);
        }

        [HttpPost, ActionName("BlogDelete")]
        [ValidateAntiForgeryToken]
        public IActionResult BlogDeleteConfirmed(Guid id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var blog = _blogService.GetById(id);
            if (blog != null)
            {
                _blogService.Delete(blog);
                TempData["Success"] = "Blog deleted successfully.";
            }
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            if (id == null)
            {
                return NotFound();
            }

            var booking = await _bookingService.GetBookingByIdAsync(id.Value);
            if (booking == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/EditBooking.cshtml", booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingDate,CustomerId,ConsultantId,StartDate,Status,Phone")] Booking booking)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bookingService.UpdateBookingAsync(booking);
                    TempData["Success"] = "Booking updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BookingExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Dashboard));
            }
            return View("~/Views/Admin/EditBooking.cshtml", booking);
        }

        private async Task<bool> BookingExists(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            return booking != null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var courses = await _courseService.GetAllCoursesWithLessonsAsync();
            var activeCount = courses.Count(c => c.IsActive);
            var inactiveCount = courses.Count(c => !c.IsActive);
            System.Diagnostics.Debug.WriteLine($"Active courses: {activeCount}, Inactive: {inactiveCount}");
            return View("~/Views/Admin/Index.cshtml", courses);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var course = await _courseService.GetCourseWithLessonsAsync(id);
            if (course == null) return NotFound();

            return View("~/Views/Admin/Details.cshtml", course);
        }

        [HttpGet("lessons/{courseId}")]
        public async Task<IActionResult> LessonIndex(Guid courseId)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);

            var lessonDTOs = lessons.Select(l => new LessonDTO
            {
                LessonId = l.LessonId,
                CourseId = l.CourseId,
                Title = l.Title,
                ContentType = l.ContentType,
                Duration = l.Duration,
                OrderNumber = l.OrderNumber,
            }).ToList();

            ViewBag.CourseId = courseId;
            ViewBag.CourseTitle = course.Title;

            return View("~/Views/Admin/LessonIndex.cshtml", lessonDTOs);
        }

        [HttpGet("lessons/details/{courseId}/{lessonId}")]
        public async Task<IActionResult> LessonDetails(Guid courseId, Guid lessonId)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var lesson = await _lessonService.GetLessonByIdAsync(lessonId);
            if (lesson == null) return NotFound();

            var lessonDto = new LessonDTO
            {
                LessonId = lesson.LessonId,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                ContentType = lesson.ContentType,
                Duration = lesson.Duration,
                OrderNumber = lesson.OrderNumber,
            };

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            ViewBag.CourseId = courseId;
            ViewBag.CourseTitle = course.Title;

            return View("~/Views/Admin/LessonDetails.cshtml", lessonDto);
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }
    }
}