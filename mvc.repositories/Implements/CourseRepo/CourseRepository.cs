using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.Entities.Courses;
using mvc.repositories.Interfaces.ICourse;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mvc.dataaccess.ViewModels;

namespace mvc.repositories.Implements.CourseRepo
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;


        public CourseRepository(AppDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.CategoryMappings)
                    .ThenInclude(cm => cm.Category)
                .Include(c => c.Modules)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<CoursesDTO> GetCourseDTOByIdAsync(Guid courseId)
        {
            var course = await _context.Courses
                .Include(c => c.CategoryMappings)
                .Include(c => c.Prerequisites)
                .Include(c => c.Modules)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null) return null;

            return new CoursesDTO
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                Duration = course.Duration,
                DifficultyLevel = course.DifficultyLevel,
                ImageBytes = course.ImageBytes,
                ImageContentType = course.ImageContentType,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt,
                IsActive = course.IsActive,
                CategoryMappingIds = course.CategoryMappings?.Select(cm => cm.CategoryId).ToList(),
                PrerequisiteCourseIds = course.Prerequisites?.Select(p => p.PrerequisiteCourseId).ToList(),
                ModuleIds = course.Modules?.Select(m => m.ModuleId).ToList()
            };
        }

        public async Task<Course> GetCourseByIdAsync(Guid courseId)
        {
            return await _context.Courses
                .Include(c => c.CategoryMappings)
                    .ThenInclude(cm => cm.Category)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .Include(c => c.Prerequisites)
                    .ThenInclude(p => p.PrerequisiteCourse)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<CoursesDTO> CreateCourseFromDTOAsync(CoursesDTO courseDTO, IFormFile imageFile = null)
        {
            var course = new Course
            {
                CourseId = Guid.NewGuid(), // Explicitly set the ID
                Title = courseDTO.Title,
                Description = courseDTO.Description,
                Duration = courseDTO.Duration,
                DifficultyLevel = courseDTO.DifficultyLevel,
                IsActive = courseDTO.IsActive, // Ensure this is set
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Process image file if provided
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageData = await ProcessImageFileAsync(imageFile);
                course.ImageBytes = imageData.ImageBytes;
                course.ImageContentType = imageData.ContentType;
            }

            // Log the values before saving
            Console.WriteLine($"Creating course with IsActive: {course.IsActive}");

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            // Update the DTO with the generated ID and return values
            courseDTO.CourseId = course.CourseId;
            courseDTO.CreatedAt = course.CreatedAt;
            courseDTO.UpdatedAt = course.UpdatedAt;

            return courseDTO;
        }

        public async Task<CoursesDTO> UpdateCourseFromDTOAsync(CoursesDTO courseDTO, IFormFile imageFile = null)
        {
            var existingCourse = await _context.Courses
                .Include(c => c.CategoryMappings)
                .Include(c => c.Prerequisites)
                .FirstOrDefaultAsync(c => c.CourseId == courseDTO.CourseId);

            if (existingCourse == null)
                throw new KeyNotFoundException($"Course with ID {courseDTO.CourseId} not found");

            // Process and update image if new one was provided
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageData = await ProcessImageFileAsync(imageFile);
                existingCourse.ImageBytes = imageData.ImageBytes;
                existingCourse.ImageContentType = imageData.ContentType;
            }

            // Update scalar properties
            existingCourse.Title = courseDTO.Title;
            existingCourse.Description = courseDTO.Description;
            existingCourse.Duration = courseDTO.Duration;
            existingCourse.DifficultyLevel = courseDTO.DifficultyLevel;
            existingCourse.IsActive = courseDTO.IsActive;
            existingCourse.UpdatedAt = DateTime.UtcNow;

            // Update categories if they exist in DTO
            if (courseDTO.CategoryMappingIds != null)
            {
                var newMappings = courseDTO.CategoryMappingIds.Select(id => new CourseCategoryMapping
                {
                    CourseId = existingCourse.CourseId,
                    CategoryId = id
                }).ToList();
                UpdateCourseCategories(existingCourse, newMappings);
            }

            // Update prerequisites if they exist in DTO
            if (courseDTO.PrerequisiteCourseIds != null)
            {
                var newPrerequisites = courseDTO.PrerequisiteCourseIds.Select(id => new CoursePrerequisite
                {
                    CourseId = existingCourse.CourseId,
                    PrerequisiteCourseId = id
                }).ToList();
                UpdateCoursePrerequisites(existingCourse, newPrerequisites);
            }

            await _context.SaveChangesAsync();

            return courseDTO;
        }

        public async Task<byte[]> GetCourseImageAsync(Guid courseId)
        {
            return await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => c.ImageBytes)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetCourseImageContentTypeAsync(Guid courseId)
        {
            return await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => c.ImageContentType)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteCourseAsync(Guid courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(Guid categoryId)
        {
            return await _context.CourseCategoryMappings
                .Where(cc => cc.CategoryId == categoryId)
                .Select(cc => cc.Course)
                .Include(c => c.CategoryMappings)
                    .ThenInclude(cm => cm.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetActiveCoursesAsync()
        {
            return await _context.Courses
                .Where(c => c.IsActive)
                .Include(c => c.CategoryMappings)
                    .ThenInclude(cm => cm.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> CourseExistsAsync(Guid courseId)
        {
            return await _context.Courses.AnyAsync(c => c.CourseId == courseId);
        }

        #region Private Helper Methods

        private async Task<(byte[] ImageBytes, string ContentType)> ProcessImageFileAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return (null, null);

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(imageFile.ContentType.ToLower()))
            {
                throw new ArgumentException($"Invalid file type. Allowed types: {string.Join(", ", allowedTypes)}");
            }

            // Validate file size (e.g., max 5MB)
            const int maxFileSize = 5 * 1024 * 1024; // 5MB
            if (imageFile.Length > maxFileSize)
            {
                throw new ArgumentException($"File size too large. Maximum allowed size is {maxFileSize / 1024 / 1024}MB");
            }

            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                return (memoryStream.ToArray(), imageFile.ContentType);
            }
        }

        private void UpdateCourseCategories(Course existingCourse, ICollection<CourseCategoryMapping> newMappings)
        {
            // Remove categories no longer selected
            var categoriesToRemove = existingCourse.CategoryMappings
                .Where(cm => !newMappings.Any(n => n.CategoryId == cm.CategoryId))
                .ToList();

            foreach (var categoryToRemove in categoriesToRemove)
            {
                _context.Entry(categoryToRemove).State = EntityState.Deleted;
            }

            // Add new categories
            var categoriesToAdd = newMappings
                .Where(n => !existingCourse.CategoryMappings.Any(cm => cm.CategoryId == n.CategoryId))
                .ToList();

            foreach (var categoryToAdd in categoriesToAdd)
            {
                existingCourse.CategoryMappings.Add(new CourseCategoryMapping
                {
                    CourseId = existingCourse.CourseId,
                    CategoryId = categoryToAdd.CategoryId
                });
            }
        }

        private void UpdateCoursePrerequisites(Course existingCourse, ICollection<CoursePrerequisite> newPrerequisites)
        {
            // Remove prerequisites no longer selected
            var prerequisitesToRemove = existingCourse.Prerequisites
                .Where(p => !newPrerequisites.Any(np => np.PrerequisiteCourseId == p.PrerequisiteCourseId))
                .ToList();

            foreach (var prerequisiteToRemove in prerequisitesToRemove)
            {
                _context.Entry(prerequisiteToRemove).State = EntityState.Deleted;
            }

            // Add new prerequisites
            var prerequisitesToAdd = newPrerequisites
                .Where(np => !existingCourse.Prerequisites.Any(p => p.PrerequisiteCourseId == np.PrerequisiteCourseId))
                .ToList();

            foreach (var prerequisiteToAdd in prerequisitesToAdd)
            {
                existingCourse.Prerequisites.Add(new CoursePrerequisite
                {
                    CourseId = existingCourse.CourseId,
                    PrerequisiteCourseId = prerequisiteToAdd.PrerequisiteCourseId
                });
            }
        }

        #endregion
    }
}