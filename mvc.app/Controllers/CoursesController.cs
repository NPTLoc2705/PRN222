using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities.Courses;
using mvc.services.Interfaces;
using Microsoft.Extensions.Logging;
using mvc.dataaccess.ViewModels;

namespace mvc.app.Controllers
{
    
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            try
            {
                var courses = await _courseService.GetAllCoursesAsync();
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
                if (course == null)
                {
                    return NotFound();
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
        public IActionResult Create()
        {
            return View(new CoursesDTO { IsActive = true });
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Duration,DifficultyLevel,IsActive")] CoursesDTO courseDTO,
     IFormFile imageFile)
        {
            _logger.LogInformation("=== CREATE COURSE DEBUG START ===");
            _logger.LogInformation("Course Title: {Title}", courseDTO?.Title);
            _logger.LogInformation("Course Description: {Description}", courseDTO?.Description);
            _logger.LogInformation("Course Duration: {Duration}", courseDTO?.Duration);
            _logger.LogInformation("Course DifficultyLevel: {DifficultyLevel}", courseDTO?.DifficultyLevel);
            _logger.LogInformation("Course IsActive: {IsActive}", courseDTO?.IsActive);
            _logger.LogInformation("Image file provided: {HasImage}", imageFile != null);
            _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);

            // Ensure IsActive has a default value if not set
            if (courseDTO != null && !Request.Form.ContainsKey("IsActive"))
            {
                courseDTO.IsActive = true; // Set default value
                _logger.LogInformation("IsActive not found in form, setting default to true");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors:");
                foreach (var modelError in ModelState)
                {
                    foreach (var error in modelError.Value.Errors)
                    {
                        _logger.LogWarning("Field: {Field}, Error: {Error}", modelError.Key, error.ErrorMessage);
                    }
                }
                _logger.LogInformation("=== CREATE COURSE DEBUG END (ModelState Invalid) ===");
                return View(courseDTO);
            }

            try
            {
                _logger.LogInformation("ModelState is valid, proceeding with course creation...");
                _logger.LogInformation("About to call _courseService.CreateCourseFromDTOAsync");
                _logger.LogInformation("Final IsActive value before service call: {IsActive}", courseDTO.IsActive);

                var result = await _courseService.CreateCourseFromDTOAsync(courseDTO, imageFile);
                _logger.LogInformation("Course created successfully, redirecting to Index");

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "ArgumentException during course creation");
                ModelState.AddModelError("imageFile", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during course creation");
                ModelState.AddModelError("", "An error occurred while creating the course.");
            }

            _logger.LogInformation("=== CREATE COURSE DEBUG END (Exception or Error) ===");
            return View(courseDTO);
        }

        // GET: Courses/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var courseDTO = await _courseService.GetCourseDTOByIdAsync(id.Value);
                if (courseDTO == null)
                {
                    return NotFound();
                }
                return View(courseDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course for edit with ID {CourseId}", id);
                return View("Error");
            }
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CourseId,Title,Description,Duration,DifficultyLevel,IsActive")] CoursesDTO courseDTO,
    IFormFile imageFile)
        {
            if (id != courseDTO.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _courseService.UpdateCourseFromDTOAsync(courseDTO, imageFile);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("imageFile", ex.Message);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _courseService.CourseExistsAsync(courseDTO.CourseId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating course with ID {CourseId}", id);
                    ModelState.AddModelError("", "An error occurred while updating the course.");
                }
            }
            return View(courseDTO);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var course = await _courseService.GetCourseByIdAsync(id.Value);
                if (course == null)
                {
                    return NotFound();
                }

                return View(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course for deletion with ID {CourseId}", id);
                return View("Error");
            }
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course != null)
                {
                    await _courseService.DeleteCourseAsync(id);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course with ID {CourseId}", id);
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
    }
}