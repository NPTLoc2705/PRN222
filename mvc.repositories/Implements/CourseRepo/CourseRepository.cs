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
                .Include(c => c.Lessons)
                .Where(c => c.IsActive) // Only get active courses
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<CoursesDTO> GetCourseDTOByIdAsync(Guid courseId)
        {
            var course = await _context.Courses
                .Include(c => c.CategoryMappings)
                .Include(c => c.Lessons)
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
                SelectedCategoryIds = course.CategoryMappings?.Select(cm => cm.CategoryId).ToList(),
                Lessons = course.Lessons?.Select(l => new LessonDTO
                {
                    LessonId = l.LessonId,
                    CourseId = l.CourseId, // Add CourseId mapping
                    Title = l.Title,
                    ContentType = l.ContentType,
                    ContentUrl = l.ContentUrl,
                    Duration = l.Duration,
                    OrderNumber = l.OrderNumber,
                    IsFreePreview = l.IsFreePreview,
                    CreatedAt = l.CreatedAt
                }).ToList()
            };
        }

        public async Task<Course> GetCourseByIdAsync(Guid courseId)
        {
            return await _context.Courses
                .Include(c => c.CategoryMappings)
                    .ThenInclude(cm => cm.Category)
                .Include(c => c.Lessons)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<CoursesDTO> CreateCourseFromDTOAsync(CoursesDTO courseDTO, IFormFile imageFile = null)
        {
            var course = new Course
            {
                CourseId = Guid.NewGuid(),
                Title = courseDTO.Title,
                Description = courseDTO.Description,
                Duration = courseDTO.Duration,
                DifficultyLevel = courseDTO.DifficultyLevel,
                IsActive = courseDTO.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CategoryMappings = new List<CourseCategoryMapping>(), // Initialize here
                Lessons = new List<Lesson>() // Initialize here
            };

            if (imageFile != null && imageFile.Length > 0)
            {
                var imageData = await ProcessImageFileAsync(imageFile);
                course.ImageBytes = imageData.ImageBytes;
                course.ImageContentType = imageData.ContentType;
            }

            if (courseDTO.SelectedCategoryIds != null && courseDTO.SelectedCategoryIds.Any())
            {
                foreach (var categoryId in courseDTO.SelectedCategoryIds)
                {
                    course.CategoryMappings.Add(new CourseCategoryMapping
                    {
                        CourseId = course.CourseId,
                        CategoryId = categoryId
                    });
                }
            }

            // Add lessons if they exist in DTO
            if (courseDTO.Lessons != null && courseDTO.Lessons.Any())
            {
                course.Lessons = courseDTO.Lessons.Select(l => new Lesson
                {
                    LessonId = Guid.NewGuid(),
                    Title = l.Title,
                    ContentType = l.ContentType,
                    ContentUrl = l.ContentUrl,
                    Duration = l.Duration,
                    OrderNumber = l.OrderNumber,
                    IsFreePreview = l.IsFreePreview,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            courseDTO.CourseId = course.CourseId;
            courseDTO.CreatedAt = course.CreatedAt;
            courseDTO.UpdatedAt = course.UpdatedAt;

            return courseDTO;
        }


        public async Task<CoursesDTO> UpdateCourseFromDTOAsync(CoursesDTO courseDTO, IFormFile imageFile = null)
        {
            var existingCourse = await _context.Courses
                .Include(c => c.CategoryMappings)
                .Include(c => c.Lessons)
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
            else if (courseDTO.ImageBytes != null) // Preserve existing image if no new one uploaded
            {
                existingCourse.ImageBytes = courseDTO.ImageBytes;
                existingCourse.ImageContentType = courseDTO.ImageContentType;
            }

            // Update scalar properties
            existingCourse.Title = courseDTO.Title;
            existingCourse.Description = courseDTO.Description;
            existingCourse.Duration = courseDTO.Duration;
            existingCourse.DifficultyLevel = courseDTO.DifficultyLevel;
            existingCourse.IsActive = courseDTO.IsActive;
            existingCourse.UpdatedAt = DateTime.UtcNow;

            // Update categories
            if (courseDTO.SelectedCategoryIds != null)
            {
                // Remove existing mappings not in the new list
                var categoriesToRemove = existingCourse.CategoryMappings
                    .Where(cm => !courseDTO.SelectedCategoryIds.Contains(cm.CategoryId))
                    .ToList();

                foreach (var mapping in categoriesToRemove)
                {
                    _context.CourseCategoryMappings.Remove(mapping);
                }

                // Add new mappings
                var existingCategoryIds = existingCourse.CategoryMappings.Select(cm => cm.CategoryId);
                var categoriesToAdd = courseDTO.SelectedCategoryIds
                    .Where(id => !existingCategoryIds.Contains(id))
                    .Select(id => new CourseCategoryMapping
                    {
                        CourseId = existingCourse.CourseId,
                        CategoryId = id
                    });

                await _context.CourseCategoryMappings.AddRangeAsync(categoriesToAdd);
            }

            // Update lessons
            if (courseDTO.Lessons != null)
            {
                UpdateCourseLessons(existingCourse, courseDTO.Lessons);
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
            course.IsActive = false; // Soft delete

            _context.Courses.Update(course);
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

        public async Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm)
        {
            return await _context.Courses
                .Include(c => c.CategoryMappings)
                    .ThenInclude(cm => cm.Category)
                .Where(c => c.Title.Contains(searchTerm) ||
                           c.Description.Contains(searchTerm) ||
                           c.DifficultyLevel.Contains(searchTerm))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryNameAsync(string categoryName)
        {
            return await _context.CourseCategoryMappings
                .Include(cc => cc.Category)
                .Include(cc => cc.Course)
                    .ThenInclude(c => c.CategoryMappings)
                        .ThenInclude(cm => cm.Category)
                .Where(cc => cc.Category.Name.Contains(categoryName))
                .Select(cc => cc.Course)
                .AsNoTracking()
                .ToListAsync();
        }
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
        private void UpdateCourseLessons(Course existingCourse, ICollection<LessonDTO> newLessons)
        {
            // Remove lessons not in the new list
            var lessonsToRemove = existingCourse.Lessons?
                .Where(l => !newLessons.Any(nl => nl.LessonId == l.LessonId))
                .ToList();

            if (lessonsToRemove != null)
            {
                foreach (var lesson in lessonsToRemove)
                {
                    _context.Lessons.Remove(lesson);
                }
            }

            // Update existing lessons and add new ones
            foreach (var lessonDto in newLessons)
            {
                var existingLesson = existingCourse.Lessons?
                    .FirstOrDefault(l => l.LessonId == lessonDto.LessonId);

                if (existingLesson != null)
                {
                    // Update existing lesson
                    existingLesson.Title = lessonDto.Title;
                    existingLesson.ContentType = lessonDto.ContentType;
                    existingLesson.ContentUrl = lessonDto.ContentUrl;
                    existingLesson.Duration = lessonDto.Duration;
                    existingLesson.OrderNumber = lessonDto.OrderNumber;
                    existingLesson.IsFreePreview = lessonDto.IsFreePreview;
                }
                else
                {
                    // Add new lesson
                    existingCourse.Lessons.Add(new Lesson
                    {
                        LessonId = Guid.NewGuid(),
                        CourseId = existingCourse.CourseId, // Ensure CourseId is set
                        Title = lessonDto.Title,
                        ContentType = lessonDto.ContentType,
                        ContentUrl = lessonDto.ContentUrl,
                        Duration = lessonDto.Duration,
                        OrderNumber = lessonDto.OrderNumber,
                        IsFreePreview = lessonDto.IsFreePreview,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        public async Task<IEnumerable<Course>> GetAllCoursesWithLessonsAsync()
        {
            return await _context.Courses
       .Include(c => c.Lessons)
       .Include(c => c.CategoryMappings)
       .ThenInclude(cm => cm.Category)
       .AsNoTracking()
       .ToListAsync();
        }

        public async Task<Course> GetCourseWithLessonsAsync(Guid id)
        {
            return await _context.Courses
        .Include(c => c.Lessons)
        .Include(c => c.CategoryMappings)
        .ThenInclude(cm => cm.Category)
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.CourseId == id);
        }
    }
}