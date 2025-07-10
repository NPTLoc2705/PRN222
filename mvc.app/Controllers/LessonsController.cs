// Change route and remove IModuleService
using Microsoft.AspNetCore.Mvc;
using mvc.dataaccess.ViewModels;
using mvc.services.Interfaces;

[Route("courses/{courseId}/[controller]")]
public class LessonsController : Controller
{
    private readonly ILessonService _lessonService;
    private readonly IProgressService _progressService;
    private readonly ICourseService _courseService;
    private readonly ILogger<LessonsController> _logger;

    public LessonsController(ILessonService lessonService, IProgressService progressService, ICourseService courseService, ILogger<LessonsController> logger)
    {
        _lessonService = lessonService;
        _progressService = progressService;
        _courseService = courseService;
        _logger = logger;
    }

    // Update all actions to work with courseId instead of moduleId
    [HttpGet]
    public async Task<IActionResult> Index(Guid courseId)
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return Unauthorized("User not logged in or invalid session");
        }

        var course = await _courseService.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return NotFound();
        }

        var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
        var completedLessons = await _progressService.GetCompletedLessonIds(userId, courseId);

        // Convert to DTOs
        var lessonDTOs = lessons.Select(l => new LessonDTO
        {
            LessonId = l.LessonId,
            CourseId = l.CourseId,
            Title = l.Title,
            ContentType = l.ContentType,
            ContentUrl = l.ContentUrl,
            Duration = l.Duration,
            OrderNumber = l.OrderNumber,
            IsFreePreview = l.IsFreePreview,
            CreatedAt = l.CreatedAt
        }).ToList();

        ViewBag.CourseId = courseId;
        ViewBag.Course = course;
        ViewBag.CompletedLessons = completedLessons;
        return View(lessonDTOs);
    }

    [HttpGet("details/{lessonId}")]
    public async Task<IActionResult> Details(Guid courseId, Guid lessonId)
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");

        var userId = Guid.Parse(userIdString);

        var lesson = await _lessonService.GetLessonByIdAsync(lessonId);
        if (lesson == null || lesson.CourseId != courseId) return NotFound();

        // Ensure these values are being set
        ViewBag.IsUserEnrolled = await _progressService.IsUserEnrolled(userId, courseId);
        ViewBag.IsLessonCompleted = await _progressService.IsLessonCompleted(userId, courseId, lessonId);
        ViewBag.CourseProgressPercentage = await _progressService.GetCourseProgressPercentage(userId, courseId);

        return View(lesson);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(Guid courseId)
    {
        var course = await _courseService.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return NotFound();
        }

        ViewBag.CourseId = courseId;
        ViewBag.CourseTitle = course.Title;
        return View(new LessonDTO { CourseId = courseId });
    }
    [HttpPost("create")]
[ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid courseId, LessonDTO lessonDto)
    {
        // Check if user is admin/instructor
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole != "Member")
        {
            return Unauthorized();
        }

        if (courseId != lessonDto.CourseId)
        {
            return BadRequest("Course ID mismatch");
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Process file upload if present
                if (lessonDto.ContentFile != null && lessonDto.ContentFile.Length > 0)
                {
                    var filePath = await SaveLessonContent(lessonDto.ContentFile);
                    lessonDto.ContentUrl = filePath;
                }

                var result = await _lessonService.CreateLessonAsync(lessonDto);

                if (result.Error)
                {
                    ModelState.AddModelError("", result.Message);
                }
                else
                {
                    TempData["SuccessMessage"] = "Lesson created successfully!";
                    return RedirectToAction(nameof(Index), new { courseId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lesson");
                ModelState.AddModelError("", "An error occurred while creating the lesson.");
            }
        }

        ViewBag.CourseId = courseId;
        ViewBag.CourseTitle = (await _courseService.GetCourseByIdAsync(courseId))?.Title;
        return View(lessonDto);
    }
    private async Task<string> SaveLessonContent(IFormFile file)
    {
        // Implement your file storage logic here
        // This could save to wwwroot, cloud storage, etc.
        // Example for local storage:
        var uploadsFolder = Path.Combine("wwwroot", "lesson-content");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/lesson-content/{uniqueFileName}";
    }

    // Similarly update Edit, Delete, and Reorder actions to use courseId instead of moduleId
    // Remove all module-related checks and references
}