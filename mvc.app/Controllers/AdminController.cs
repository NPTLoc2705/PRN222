using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;
using mvc.services.Implements;
using mvc.services.Interfaces;

namespace mvc.app.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IBookingService _bookingService;
        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;
        private readonly IProgressService _progressService;
        public AdminController(IBookingService bookingService, IAuthService userService, ICourseService courseService,
     ILessonService lessonService,
     IProgressService progressService)
        {
            _bookingService = bookingService;
            _courseService = courseService;
            _lessonService = lessonService;
            _progressService = progressService;

        }
        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("Role");            // This action could be used to display admin dashboard information
            var customerIdObj = HttpContext.Session.GetString("UserId");
            if (customerIdObj == null || role == "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }
            var list = await _bookingService.GetAllBookingsWithNamesAsync();
            return View("AdminPage",list);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _bookingService.GetBookingByIdAsync(id.Value);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingDate,CustomerId,ConsultantId,StartDate,Status,Phone")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bookingService.UpdateBookingAsync(booking);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }
        private bool BookingExists(int id)
        {
            var booking = _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var courses = await _courseService.GetAllCoursesWithLessonsAsync(); // New service method
            var activeCount = courses.Count(c => c.IsActive);
            var inactiveCount = courses.Count(c => !c.IsActive);
            System.Diagnostics.Debug.WriteLine($"Active courses: {activeCount}, Inactive: {inactiveCount}");
            return View("~/Views/Admin/Index.cshtml", courses);
        }

        // Manager Details View
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var course = await _courseService.GetCourseWithLessonsAsync(id); // New service method
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

            // Convert to DTOs
            var lessonDTOs = lessons.Select(l => new LessonDTO
            {
                LessonId = l.LessonId,
                CourseId = l.CourseId,
                Title = l.Title,
                ContentType = l.ContentType,
                Duration = l.Duration,
                OrderNumber = l.OrderNumber,
                // Add other properties as needed
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

            // Convert to DTO
            var lessonDto = new LessonDTO
            {
                LessonId = lesson.LessonId,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                ContentType = lesson.ContentType,
                Duration = lesson.Duration,
                OrderNumber = lesson.OrderNumber,
                // Add other properties as needed
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
