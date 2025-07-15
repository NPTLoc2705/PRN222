using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.Entities.Courses;
using mvc.dataaccess.ViewModels;
using mvc.services.Interfaces;
using static mvc.dataaccess.ViewModels.CategoryDTO;

namespace mvc.app.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return View("Error");
            }
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id.Value);
                if (category == null) return NotFound();

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category details");
                return View("Error");
            }
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View(new CreateCategoryDto());
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                if (await _categoryService.CategoryNameExistsAsync(dto.Name))
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(dto);
                }

                var category = new CourseCategory
                {
                    CategoryId = Guid.NewGuid(),
                    Name = dto.Name,
                    CourseMappings = new List<CourseCategoryMapping>()
                };

                await _categoryService.CreateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                ModelState.AddModelError("", "An error occurred while creating the category.");
                return View(dto);
            }
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id.Value);
                if (category == null) return NotFound();

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category for edit");
                return View("Error");
            }
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CategoryId,Name")] CourseCategory category)
        {
            if (id != category.CategoryId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if category name already exists (excluding current category)
                    if (await _categoryService.CategoryNameExistsAsync(category.Name, id))
                    {
                        ModelState.AddModelError("Name", "A category with this name already exists.");
                        return View(category);
                    }

                    await _categoryService.UpdateCategoryAsync(category);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating category");
                    ModelState.AddModelError("", "An error occurred while updating the category.");
                }
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id.Value);
                if (category == null) return NotFound();

                // Check if category is in use
                var isInUse = category.CourseMappings?.Any() ?? false;
                ViewBag.CanDelete = !isInUse;

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category for deletion");
                return View("Error");
            }
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = "Could not delete category. It may be in use by one or more courses.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return View("Error");
            }
        }
    }

}
