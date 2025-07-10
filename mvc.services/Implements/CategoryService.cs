using mvc.dataaccess.Entities.Courses;
using mvc.repositories.Interfaces.ICourse;
using mvc.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Implements
{
    public class CategoryService : ICategoryService 
    {
        private readonly ICategoryRepo _categoryRepo;
        public CategoryService(ICategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public Task<bool> CategoryExistsAsync(Guid categoryId)
        => _categoryRepo.CategoryExistsAsync(categoryId);

        public Task<bool> CategoryNameExistsAsync(string name, Guid? excludeId = null)
        => _categoryRepo.CategoryNameExistsAsync(name, excludeId);

        public Task<CourseCategory> CreateCategoryAsync(CourseCategory category)
        => _categoryRepo.CreateCategoryAsync(category);

        public Task<bool> DeleteCategoryAsync(Guid categoryId)
        => _categoryRepo.DeleteCategoryAsync(categoryId);

        public Task DeleteCategoryMappingsForCourseAsync(Guid courseId)
        => _categoryRepo.DeleteCategoryMappingsForCourseAsync(courseId);

        public Task<IEnumerable<CourseCategory>> GetAllCategoriesAsync()
        => _categoryRepo.GetAllCategoriesAsync();

        public Task<CourseCategory> GetCategoryByIdAsync(Guid categoryId)
        => _categoryRepo.GetCategoryByIdAsync(categoryId);

        public Task<CourseCategory> UpdateCategoryAsync(CourseCategory category)
        => _categoryRepo.UpdateCategoryAsync(category);
    }
}
