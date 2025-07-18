using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.Entities.Courses;
using mvc.dataaccess.ViewModels;
using mvc.repositories.Interfaces.ICourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Implements.CourseRepo
{
    public class LessonRepository : ILessonRepository
    {
        private readonly AppDbContext _context;

        public LessonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LessonDTO> GetLessonByIdAsync(Guid lessonId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);

            if (lesson == null) return null;

            return new LessonDTO
            {
                LessonId = lesson.LessonId,
                CourseId = lesson.CourseId, // Changed from ModuleId to CourseId
                Title = lesson.Title,
                ContentType = lesson.ContentType,
                ContentUrl = lesson.ContentUrl,
                Duration = lesson.Duration,
                OrderNumber = lesson.OrderNumber,
                IsFreePreview = lesson.IsFreePreview,
                CreatedAt = lesson.CreatedAt,
                CourseTitle = lesson.Course?.Title // Changed from ModuleTitle to CourseTitle
            };
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.OrderNumber)
                .ToListAsync();
        }


        public async Task<LessonDTO> CreateLessonAsync(LessonDTO lessonDto)
        {
            // Validate course exists
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == lessonDto.CourseId);
            if (!courseExists)
            {
                return new LessonDTO { Error = true, Message = "Course not found" };
            }

            // Get the next order number
            var maxOrder = await _context.Lessons
                .Where(l => l.CourseId == lessonDto.CourseId)
                .MaxAsync(l => (int?)l.OrderNumber) ?? 0;

            var lesson = new Lesson
            {
                LessonId = Guid.NewGuid(),
                CourseId = lessonDto.CourseId, 
                Title = lessonDto.Title,
                ContentType = lessonDto.ContentType,
                ContentUrl = lessonDto.ContentUrl,
                Duration = lessonDto.Duration,
                OrderNumber = maxOrder + 1,
                IsFreePreview = lessonDto.IsFreePreview,
                CreatedAt = DateTime.UtcNow
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return new LessonDTO
            {
                LessonId = lesson.LessonId,
                CourseId = lesson.CourseId, 
                Title = lesson.Title,
                ContentType = lesson.ContentType,
                ContentUrl = lesson.ContentUrl,
                Duration = lesson.Duration,
                OrderNumber = lesson.OrderNumber,
                IsFreePreview = lesson.IsFreePreview,
                CreatedAt = lesson.CreatedAt
            };
        }

        public async Task<LessonDTO> UpdateLessonAsync(LessonDTO lessonDto)
        {
            var lesson = await _context.Lessons.FindAsync(lessonDto.LessonId);
            if (lesson == null)
            {
                return new LessonDTO { Error = true, Message = "Lesson not found" };
            }

            lesson.Title = lessonDto.Title;
            lesson.ContentType = lessonDto.ContentType;
            lesson.ContentUrl = lessonDto.ContentUrl;
            lesson.Duration = lessonDto.Duration;
            lesson.OrderNumber = lessonDto.OrderNumber;
            lesson.IsFreePreview = lessonDto.IsFreePreview;

            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();

            return lessonDto;
        }

        public async Task<bool> DeleteLessonAsync(Guid lessonId)
        {
            var lesson = await _context.Lessons
        .Include(l => l.UserProgresses) // Include related progress records
        .FirstOrDefaultAsync(l => l.LessonId == lessonId);

            if (lesson == null) return false;

            // Remove all related progress records
            _context.UserCourseProgresses.RemoveRange(lesson.UserProgresses);

            // Then remove the lesson
            _context.Lessons.Remove(lesson);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReorderLessonsAsync(Guid courseId, List<Guid> orderedLessonIds)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .ToListAsync();

            if (lessons.Count != orderedLessonIds.Count)
            {
                return false;
            }

            for (int i = 0; i < orderedLessonIds.Count; i++)
            {
                var lesson = lessons.FirstOrDefault(l => l.LessonId == orderedLessonIds[i]);
                if (lesson != null)
                {
                    lesson.OrderNumber = i + 1;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ProcessLessonContentFileAsync(Guid lessonId, IFormFile contentFile)
        {
            var lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null) return false;

            // Process file upload logic here
            // Example: save to storage and update ContentUrl
            // lesson.ContentUrl = processedFileUrl;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteLessonsByCourseIdAsync(Guid courseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .ToListAsync();

            if (lessons.Any())
            {
                _context.Lessons.RemoveRange(lessons);
                await _context.SaveChangesAsync();
            }
        }
    }
}

