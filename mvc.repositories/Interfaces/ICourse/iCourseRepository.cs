using Microsoft.AspNetCore.Http;
using mvc.dataaccess.Entities.Courses;
using mvc.dataaccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mvc.repositories.Interfaces.ICourse
{
    public interface ICourseRepository
    {
        // Existing methods
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course> GetCourseByIdAsync(Guid courseId);
        Task<bool> DeleteCourseAsync(Guid courseId);
        Task<IEnumerable<Course>> GetCoursesByCategoryAsync(Guid categoryId);
        Task<IEnumerable<Course>> GetActiveCoursesAsync();
        Task<byte[]> GetCourseImageAsync(Guid courseId);
        Task<string> GetCourseImageContentTypeAsync(Guid courseId);
        Task<bool> CourseExistsAsync(Guid courseId);


        // New DTO-based methods
        Task<CoursesDTO> GetCourseDTOByIdAsync(Guid courseId);
        Task<CoursesDTO> CreateCourseFromDTOAsync(CoursesDTO courseDTO, IFormFile imageFile = null);
        Task<CoursesDTO> UpdateCourseFromDTOAsync(CoursesDTO courseDTO, IFormFile imageFile = null);
    }
}