using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities.Courses;
using mvc.services.Implements;
using mvc.services.Interfaces;

namespace mvc.app.Controllers
{
    [Route("courses/{courseId}/[controller]")]
    public class ProgressesController : Controller
    {
        private readonly IProgressService _progressService;
        private readonly ILessonService _lessonService;
        private readonly ILogger<ProgressesController> _logger;

        public ProgressesController(IProgressService progressService, ILessonService lessonService, ILogger<ProgressesController> logger)
        {
            _progressService = progressService;
            _lessonService = lessonService;
            _logger = logger;
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

        [HttpPost("MarkLessonComplete")]
        public async Task<IActionResult> MarkLessonComplete(Guid courseId, Guid lessonId)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Verify lesson belongs to course
                var lesson = await _lessonService.GetLessonByIdAsync(lessonId);
                if (lesson == null || lesson.CourseId != courseId)
                {
                    return NotFound();
                }

                await _progressService.UpdateLessonProgress(userId, courseId, lessonId, true);

                // Return to the lesson page with success message
                TempData["SuccessMessage"] = "Lesson marked as complete!";
                return RedirectToAction("Details", "Lessons", new { courseId, lessonId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking lesson complete");
                TempData["ErrorMessage"] = "Error updating lesson progress";
                return RedirectToAction("Details", "Lessons", new { courseId, lessonId });
            }
        }

        [HttpPost("MarkLessonIncomplete")]
        public async Task<IActionResult> MarkLessonIncomplete(Guid courseId, Guid lessonId)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Verify lesson belongs to course
                var lesson = await _lessonService.GetLessonByIdAsync(lessonId);
                if (lesson == null || lesson.CourseId != courseId)
                {
                    return NotFound();
                }

                await _progressService.UpdateLessonProgress(userId, courseId, lessonId, false);

                // Return to the lesson page with success message
                TempData["SuccessMessage"] = "Lesson marked as incomplete!";
                return RedirectToAction("Details", "Lessons", new { courseId, lessonId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking lesson incomplete");
                TempData["ErrorMessage"] = "Error updating lesson progress";
                return RedirectToAction("Details", "Lessons", new { courseId, lessonId });
            }
        }
    }
}