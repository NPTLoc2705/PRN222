using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.Entities.Courses;
using mvc.repositories.Interfaces.ICourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Implements.CourseRepo
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly AppDbContext _context;
        public CategoryRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CategoryExistsAsync(Guid categoryId)
        {
            return await _context.CourseCategories.AnyAsync(c => c.CategoryId == categoryId);
        }

        public async Task<bool> CategoryNameExistsAsync(string name, Guid? excludeId = null)
        {
            return await _context.CourseCategories
                .AnyAsync(c => c.Name == name && (!excludeId.HasValue || c.CategoryId != excludeId.Value));
        }

        public async Task<CourseCategory> CreateCategoryAsync(CourseCategory category)
        {
            await _context.CourseCategories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _context.CourseCategories.FindAsync(categoryId);
            if (category == null) return false;

            // Delete all mappings for this category first
            var mappings = await _context.CourseCategoryMappings
                .Where(cc => cc.CategoryId == categoryId)
                .ToListAsync();

            if (mappings.Any())
            {
                _context.CourseCategoryMappings.RemoveRange(mappings);
            }

            _context.CourseCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteCategoryMappingsForCourseAsync(Guid courseId)
        {
             var mappings = await _context.CourseCategoryMappings
        .Where(cc => cc.CourseId == courseId)
        .ToListAsync();

    if (mappings.Any())
    {
        _context.CourseCategoryMappings.RemoveRange(mappings);
        await _context.SaveChangesAsync();
    }
        }

        public async Task<IEnumerable<CourseCategory>> GetAllCategoriesAsync()
        {
            return await _context.CourseCategories
                .Include(c => c.CourseMappings)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CourseCategory> GetCategoryByIdAsync(Guid categoryId)
        {
            return await _context.CourseCategories
                .Include(c => c.CourseMappings)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<CourseCategory> UpdateCategoryAsync(CourseCategory category)
        {
            _context.CourseCategories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}

