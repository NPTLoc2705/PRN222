using mvc.dataaccess.Entities.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CourseCategory>> GetAllCategoriesAsync();
        Task<CourseCategory> GetCategoryByIdAsync(Guid categoryId);
        Task<CourseCategory> CreateCategoryAsync(CourseCategory category);
        Task<CourseCategory> UpdateCategoryAsync(CourseCategory category);
        Task<bool> DeleteCategoryAsync(Guid categoryId);
        Task<bool> CategoryExistsAsync(Guid categoryId);
        Task<bool> CategoryNameExistsAsync(string name, Guid? excludeId = null);
        Task DeleteCategoryMappingsForCourseAsync(Guid courseId);

    }
}
