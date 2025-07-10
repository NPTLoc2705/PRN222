using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities.Courses;
using mvc.services.Interfaces;
using Microsoft.Extensions.Logging;
using mvc.dataaccess.ViewModels;
using mvc.services.Implements;

namespace mvc.app.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IProgressService _progressService;
        private readonly ILessonService _lessonService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(
            ICourseService courseService,
            ILogger<CoursesController> logger,
            ILessonService lessonService, IProgressService progressService, ICategoryService categoryService)
        {
            _courseService = courseService;
            _logger = logger;
            _lessonService = lessonService;
            _progressService = progressService;
            _categoryService = categoryService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new InvalidOperationException("User ID not found in session or invalid");
            }
            return userId;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(Guid courseId)
        {
            try
            {
                if (!await _courseService.CourseExistsAsync(courseId))
                {
                    return NotFound();
                }

                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Details", new { id = courseId }) });
                }

                // Check if already enrolled
                if (!await _progressService.IsUserEnrolled(userId, courseId))
                {
                    await _progressService.CreateProgressRecord(new UserCourseProgress
                    {
                        UserId = userId,
                        CourseId = courseId,
                        LessonId = null,
                        IsCompleted = false,
                        ProgressPercentage = 0,
                        LastAccessed = DateTime.UtcNow
                    });

                    TempData["SuccessMessage"] = "You have successfully enrolled in this course!";
                }
                else
                {
                    TempData["InfoMessage"] = "You are already enrolled in this course.";
                }

                return RedirectToAction("Details", new { id = courseId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling in course");
                TempData["ErrorMessage"] = "An error occurred while enrolling in the course.";
                return RedirectToAction("Details", new { id = courseId });
            }
        }



        // GET: Courses
        public async Task<IActionResult> Index(string searchTerm, string categoryName)
        {
            try
            {
                IEnumerable<Course> courses;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    courses = await _courseService.SearchCoursesAsync(searchTerm);
                    ViewBag.SearchTerm = searchTerm;
                }
                else if (!string.IsNullOrEmpty(categoryName))
                {
                    courses = await _courseService.GetCoursesByCategoryNameAsync(categoryName);
                    ViewBag.CategoryName = categoryName;
                }
                else
                {
                    courses = await _courseService.GetAllCoursesAsync();
                }

                var userIdString = HttpContext.Session.GetString("UserId");
                var enrollmentStatus = new Dictionary<Guid, bool>();

                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
                {
                    foreach (var course in courses)
                    {
                        enrollmentStatus[course.CourseId] = await _progressService.IsUserEnrolled(userId, course.CourseId);
                    }
                }

                ViewBag.EnrollmentStatus = enrollmentStatus;
                var categories = _categoryService.GetAllCategoriesAsync().Result; // Consider using async properly in production code
                ViewBag.Categories = new SelectList(categories, "Name", "Name");
                return View(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses");
                return View("Error");
            }
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var course = await _courseService.GetCourseByIdAsync(id.Value);
                if (course == null) return NotFound();

                var userIdString = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
                {
                    ViewBag.IsEnrolled = await _progressService.IsUserEnrolled(userId, id.Value);

                    if (ViewBag.IsEnrolled)
                    {
                        // Only load lessons if user is enrolled
                        var lessons = await _lessonService.GetLessonsByCourseIdAsync(id.Value);
                        course.Lessons = lessons.ToList();

                        ViewBag.ProgressPercentage = await _progressService.GetCourseProgressPercentage(userId, id.Value);
                        ViewBag.CompletedLessonIds = await _progressService.GetCompletedLessonIds(userId, id.Value);
                    }
                }

                return View(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course details for ID {CourseId}", id);
                return View("Error");
            }
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            var dto = new CoursesDTO
            {
                AvailableCategories = await GetCategoryDTOs()
            };
            return View(dto);
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
      [Bind("Title,Description,Duration,DifficultyLevel,IsActive,SelectedCategoryIds")] CoursesDTO courseDTO,
      IFormFile imageFile)
        {
            courseDTO.AvailableCategories = await GetCategoryDTOs();
            courseDTO.SelectedCategoryIds ??= new List<Guid>();

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                ModelState.AddModelError("", "User ID is not valid.");
                return View(courseDTO);
            }

            if (!ModelState.IsValid)
            {
                return View(courseDTO);
            }

            try
            {
                await _courseService.CreateCourseFromDTOAsync(courseDTO, imageFile);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                ModelState.AddModelError("", "An error occurred while creating the course.");
                return View(courseDTO);
            }
        }

        // GET: Courses/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var courseDTO = await _courseService.GetCourseDTOByIdAsync(id.Value);
                if (courseDTO == null) return NotFound();

                courseDTO.AvailableCategories = await GetCategoryDTOs();
                return View(courseDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course for edit");
                return View("Error");
            }
        }

        // POST: Courses/Edit/5
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    Guid id,
    [Bind("CourseId,Title,Description,Duration,DifficultyLevel,IsActive,SelectedCategoryIds")] CoursesDTO courseDTO,
    IFormFile imageFile = null)
        {
            if (id != courseDTO.CourseId) return NotFound();

            // Load available categories for the view
            courseDTO.AvailableCategories = await GetCategoryDTOs();

            // Initialize if null to avoid null reference
            courseDTO.SelectedCategoryIds ??= new List<Guid>();

            // Debug: Log model values
            _logger.LogInformation("Editing course: {@CourseDTO}", courseDTO);

            // Special handling for categories - remove validation if not needed
            if (courseDTO.SelectedCategoryIds.Count == 0)
            {
                ModelState.Remove("SelectedCategoryIds");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing course to preserve image data if no new image is uploaded
                    var existingCourse = await _courseService.GetCourseByIdAsync(id);
                    if (existingCourse != null)
                    {
                        // If no new image is uploaded, preserve the existing image data
                        if (imageFile == null || imageFile.Length == 0)
                        {
                            courseDTO.ImageBytes = existingCourse.ImageBytes;
                            courseDTO.ImageContentType = existingCourse.ImageContentType;
                        }
                    }

                    await _courseService.UpdateCourseFromDTOAsync(courseDTO, imageFile);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating course");
                    ModelState.AddModelError("", "An error occurred while updating the course.");
                }
            }
            else
            {
                var errors = ModelState
           .Where(x => x.Value.Errors.Count > 0)
           .Select(x => new { x.Key, x.Value.Errors })
           .ToList();

                _logger.LogError("ModelState errors: {@Errors}", errors);

                // Or inspect in debugger
                var errorList = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errorList)
                {
                    _logger.LogError(error.ErrorMessage);
                }
            }
            return View(courseDTO);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var course = await _courseService.GetCourseByIdAsync(id.Value);
                if (course == null) return NotFound();

                // Check for dependent records
                var hasLessons = course.Lessons?.Any() ?? false;
                var hasProgress = await _progressService.HasCourseProgress(id.Value);

                ViewBag.CanDelete = !hasLessons && !hasProgress;
                return View(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course for deletion");
                return View("Error");
            }
        }

        // POST: Courses/Delete/5
        [HttpPost("Courses/DeleteConfirmed/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                if (!await _courseService.CourseExistsAsync(id))
                {
                    return NotFound();
                }

                // Delete dependent records in proper order
                await _progressService.DeleteProgressByCourseIdAsync(id);
                await _lessonService.DeleteLessonsByCourseIdAsync(id);
                await _categoryService.DeleteCategoryMappingsForCourseAsync(id); // Use the new method

                // Then delete the course
                var result = await _courseService.DeleteCourseAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Failed to delete course {CourseId}", id);
                    return View("Error");
                }

                TempData["SuccessMessage"] = "Course deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course {CourseId}", id);
                return View("Error");
            }
        }

        // GET: Courses/Image/5
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetCourseImage(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course?.ImageBytes == null || course.ImageBytes.Length == 0)
            {
                return NotFound();
            }

            return File(course.ImageBytes, course.ImageContentType ?? "image/jpeg");
        }

        private async Task<bool> CourseExistsAsync(Guid id)
        {
            try
            {
                return await _courseService.CourseExistsAsync(id);
            }
            catch
            {
                return false;
            }
        }
        private async Task<List<CategoryDTO>> GetCategoryDTOs()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                Name = c.Name
            }).ToList();
        }
    }
}