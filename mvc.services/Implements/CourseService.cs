using Microsoft.AspNetCore.Http;
using mvc.dataaccess.Entities.Courses;
using mvc.dataaccess.ViewModels;
using mvc.repositories.Interfaces.ICourse;
using mvc.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Implements
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<bool> CourseExistsAsync(Guid courseId)
        {
            return await _courseRepository.GetCourseByIdAsync(courseId) != null;
        }

        public Task<CoursesDTO> CreateCourseFromDTOAsync(CoursesDTO courseDTO, IFormFile imageFile = null)
        => _courseRepository.CreateCourseFromDTOAsync(courseDTO, imageFile);

        public Task<bool> DeleteCourseAsync(Guid courseId)
        => _courseRepository.DeleteCourseAsync(courseId);

        public Task<IEnumerable<Course>> GetActiveCoursesAsync()
        => _courseRepository.GetActiveCoursesAsync();

        public Task<IEnumerable<Course>> GetAllCoursesAsync()
        => _courseRepository.GetAllCoursesAsync();

        public Task<Course> GetCourseByIdAsync(Guid courseId)
        => _courseRepository.GetCourseByIdAsync(courseId);

        public Task<CoursesDTO> GetCourseDTOByIdAsync(Guid courseId)
       => _courseRepository.GetCourseDTOByIdAsync(courseId);

        public Task<byte[]> GetCourseImageAsync(Guid courseId)
        => _courseRepository.GetCourseImageAsync(courseId);

        public Task<string> GetCourseImageContentTypeAsync(Guid courseId)
        => _courseRepository.GetCourseImageContentTypeAsync(courseId);

        public Task<IEnumerable<Course>> GetCoursesByCategoryAsync(Guid categoryId)
        => _courseRepository.GetCoursesByCategoryAsync(categoryId);

        public Task<CoursesDTO> UpdateCourseFromDTOAsync(CoursesDTO courseDTO, IFormFile imageFile = null)
        => _courseRepository.UpdateCourseFromDTOAsync(courseDTO, imageFile);
    }
}
