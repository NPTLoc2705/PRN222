// LessonsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities.Courses;
using mvc.dataaccess.ViewModels;
using mvc.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.app.Controllers
{
    [Route("courses/{courseId}/modules/{moduleId}/[controller]")]
    public class LessonsController : Controller
    {
        private readonly ILessonService _lessonService;
        private readonly IModuleService _moduleService;

        public LessonsController(ILessonService lessonService, IModuleService moduleService)
        {
            _lessonService = lessonService;
            _moduleService = moduleService;
        }

        // GET: courses/{courseId}/modules/{moduleId}/lessons
        [HttpGet]
        public async Task<IActionResult> Index(Guid courseId, Guid moduleId)
        {
            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            if (module == null || module.CourseId != courseId)
            {
                return NotFound();
            }

            var lessons = await _lessonService.GetLessonsByModuleIdAsync(moduleId);
            ViewBag.CourseId = courseId;
            ViewBag.Module = module;
            return View(lessons);
        }

        // GET: courses/{courseId}/modules/{moduleId}/lessons/details/{lessonId}
        [HttpGet("details/{lessonId}")]
        public async Task<IActionResult> Details(Guid courseId, Guid moduleId, Guid lessonId)
        {
            var lesson = await _lessonService.GetLessonByIdAsync(lessonId);
            if (lesson == null || lesson.ModuleId != moduleId)
            {
                return NotFound();
            }

            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            if (module == null || module.CourseId != courseId)
            {
                return NotFound();
            }

            ViewBag.CourseId = courseId;
            ViewBag.Module = module;
            return View(lesson); // Now returns LessonResponseDTO
        }

        // GET: courses/{courseId}/modules/{moduleId}/lessons/create
        [HttpGet("create")]
        public async Task<IActionResult> Create(Guid courseId, Guid moduleId)
        {
            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            if (module == null || module.CourseId != courseId)
            {
                return NotFound();
            }

            ViewBag.CourseId = courseId;
            ViewBag.Module = module;
            return View(new LessonDTO { ModuleId = moduleId });
        }

        // POST: courses/{courseId}/modules/{moduleId}/lessons/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid courseId, Guid moduleId, LessonDTO lessonDto)
        {
            if (moduleId != lessonDto.ModuleId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var result = await _lessonService.CreateLessonAsync(lessonDto);
                if (result.Error)
                {
                    ModelState.AddModelError("", result.Message);
                }
                else
                {
                    return RedirectToAction(nameof(Index), new { courseId, moduleId });
                }
            }

            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            ViewBag.CourseId = courseId;
            ViewBag.Module = module;
            return View(lessonDto);
        }

        // GET: courses/{courseId}/modules/{moduleId}/lessons/edit/{lessonId}
        [HttpGet("edit/{lessonId}")]
        public async Task<IActionResult> Edit(Guid courseId, Guid moduleId, Guid lessonId)
        {
            var lesson = await _lessonService.GetLessonByIdAsync(lessonId);
            if (lesson == null || lesson.ModuleId != moduleId)
            {
                return NotFound();
            }

            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            if (module == null || module.CourseId != courseId)
            {
                return NotFound();
            }

            ViewBag.CourseId = courseId;
            ViewBag.Module = module;

            // Convert to LessonDTO for editing
            var lessonDto = new LessonDTO
            {
                LessonId = lesson.LessonId,
                ModuleId = lesson.ModuleId,
                Title = lesson.Title,
                ContentType = lesson.ContentType,
                ContentUrl = lesson.ContentUrl,
                Duration = lesson.Duration,
                OrderNumber = lesson.OrderNumber,
                IsFreePreview = lesson.IsFreePreview,
                CreatedAt = lesson.CreatedAt
            };

            return View(lessonDto);
        }

        // POST: courses/{courseId}/modules/{moduleId}/lessons/edit/{lessonId}
        [HttpPost("edit/{lessonId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid courseId, Guid moduleId, Guid lessonId, LessonDTO lessonDto)
        {
            if (lessonId != lessonDto.LessonId || moduleId != lessonDto.ModuleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _lessonService.UpdateLessonAsync(lessonDto);
                if (result.Error)
                {
                    ModelState.AddModelError("", result.Message);
                }
                else
                {
                    return RedirectToAction(nameof(Index), new { courseId, moduleId });
                }
            }

            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            ViewBag.CourseId = courseId;
            ViewBag.Module = module;
            return View(lessonDto);
        }

        // GET: courses/{courseId}/modules/{moduleId}/lessons/delete/{lessonId}
        [HttpGet("delete/{lessonId}")]
        public async Task<IActionResult> Delete(Guid courseId, Guid moduleId, Guid lessonId)
        {
            var lesson = await _lessonService.GetLessonByIdAsync(lessonId);
            if (lesson == null || lesson.ModuleId != moduleId)
            {
                return NotFound();
            }

            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            if (module == null || module.CourseId != courseId)
            {
                return NotFound();
            }

            ViewBag.CourseId = courseId;
            ViewBag.Module = module;
            return View(lesson);
        }

        // POST: courses/{courseId}/modules/{moduleId}/lessons/delete/{lessonId}
        [HttpPost("delete/{lessonId}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid courseId, Guid moduleId, Guid lessonId)
        {
            var success = await _lessonService.DeleteLessonAsync(lessonId);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), new { courseId, moduleId });
        }

        // POST: courses/{courseId}/modules/{moduleId}/lessons/reorder
        [HttpPost("reorder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder(Guid courseId, Guid moduleId, LessonDTO reorderDto)
        {
            if (moduleId != reorderDto.ModuleId)
            {
                return BadRequest();
            }

            var success = await _lessonService.ReorderLessonsAsync(reorderDto);
            if (!success)
            {
                return BadRequest("Failed to reorder lessons");
            }

            return RedirectToAction(nameof(Index), new { courseId, moduleId });
        }
    }
}