using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.dataaccess.Entities.Courses;
using mvc.dataaccess.ViewModels;
using mvc.services.Interfaces;

namespace mvc.app.Controllers
{
    [Route("courses/{courseId}/modules")]
    public class ModulesController : Controller
    {
        private readonly IModuleService _moduleService;

        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        // GET: courses/{courseId}/modules
        [HttpGet]
        public async Task<IActionResult> Index(Guid courseId)
        {
            var modules = await _moduleService.GetModulesByCourseIdAsync(courseId);
            ViewBag.CourseId = courseId;
            return View(modules);
        }

        // GET: courses/{courseId}/modules/details/{moduleId}
        [HttpGet("details/{moduleId}")]
        public async Task<IActionResult> Details(Guid courseId, Guid moduleId)
        {
            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            if (module == null || module.CourseId != courseId)
            {
                return NotFound();
            }
            return View(module);
        }

        // GET: courses/{courseId}/modules/create
        [HttpGet("create")]
        public IActionResult Create(Guid courseId)
        {
            return View(new ModuleDTO { CourseId = courseId });
        }

        // POST: courses/{courseId}/modules/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid courseId, ModuleDTO moduleDto)
        {
            if (moduleDto.CourseId != courseId)
            {
                return BadRequest("Course mismatch");
            }

            if (ModelState.IsValid)
            {
                await _moduleService.CreateModuleAsync(moduleDto);
               return RedirectToAction("Details", "Courses", new { id = courseId });
            }
            return View(moduleDto);
        }

        // GET: courses/{courseId}/modules/edit/{moduleId}
        [HttpGet("edit/{moduleId}")]
        public async Task<IActionResult> Edit(Guid courseId, Guid moduleId)
        {
            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            if (module == null || module.CourseId != courseId)
            {
                return NotFound();
            }
            return View(module);
        }

        // POST: courses/{courseId}/modules/edit/{moduleId}
        [HttpPost("edit/{moduleId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid courseId, Guid moduleId, ModuleDTO moduleDto)
        {
            if (moduleId != moduleDto.ModuleId || courseId != moduleDto.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updatedModule = await _moduleService.UpdateModuleAsync(moduleDto);
                if (updatedModule == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index), new { courseId });
            }
            return View(moduleDto);
        }

        // GET: courses/{courseId}/modules/delete/{moduleId}
        [HttpGet("delete/{moduleId}")]
        public async Task<IActionResult> Delete(Guid courseId, Guid moduleId)
        {
            var module = await _moduleService.GetModuleByIdAsync(moduleId);
            if (module == null || module.CourseId != courseId)
            {
                return NotFound();
            }
            return View(module);
        }

        // POST: courses/{courseId}/modules/delete/{moduleId}
        [HttpPost("delete/{moduleId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid courseId, Guid moduleId)
        {
            var success = await _moduleService.DeleteModuleAsync(moduleId, courseId);
            if (!success) return NotFound();
            return RedirectToAction(nameof(Index), new { courseId });
        }
    }
}