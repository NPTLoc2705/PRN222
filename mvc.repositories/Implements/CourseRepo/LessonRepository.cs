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
                .Include(l => l.Module)
                .ThenInclude(m => m.Course)
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);

            if (lesson == null) return null;

            return new LessonDTO
            {
                LessonId = lesson.LessonId,
                ModuleId = lesson.ModuleId,
                Title = lesson.Title,
                ContentType = lesson.ContentType,
                ContentUrl = lesson.ContentUrl,
                Duration = lesson.Duration,
                OrderNumber = lesson.OrderNumber,
                IsFreePreview = lesson.IsFreePreview,
                CreatedAt = lesson.CreatedAt,
                ModuleTitle = lesson.Module?.Title,
                ModuleDescription = lesson.Module?.Description,
                CourseTitle = lesson.Module?.Course?.Title,
                CourseId = lesson.Module?.CourseId ?? Guid.Empty
            };
        }

        public async Task<IEnumerable<LessonDTO>> GetLessonsByModuleIdAsync(Guid moduleId)
        {
            return await _context.Lessons
                .Where(l => l.ModuleId == moduleId)
                .OrderBy(l => l.OrderNumber)
                .Select(l => new LessonDTO
                {
                    LessonId = l.LessonId,
                    ModuleId = l.ModuleId,
                    Title = l.Title,
                    ContentType = l.ContentType,
                    ContentUrl = l.ContentUrl,
                    Duration = l.Duration,
                    OrderNumber = l.OrderNumber,
                    IsFreePreview = l.IsFreePreview,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<LessonDTO> CreateLessonAsync(LessonDTO lessonDto)
        {
            // Validate module exists
            var moduleExists = await _context.Modules.AnyAsync(m => m.ModuleId == lessonDto.ModuleId);
            if (!moduleExists)
            {
                return new LessonDTO { Error = true, Message = "Module not found" };
            }

            // Get the next order number
            var maxOrder = await _context.Lessons
                .Where(l => l.ModuleId == lessonDto.ModuleId)
                .MaxAsync(l => (int?)l.OrderNumber) ?? 0;

            var lesson = new Lesson
            {
                LessonId = Guid.NewGuid(),
                ModuleId = lessonDto.ModuleId,
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
                ModuleId = lesson.ModuleId,
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
            var lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null) return false;

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReorderLessonsAsync(LessonDTO reorderDto)
        {
            var lessons = await _context.Lessons
                .Where(l => l.ModuleId == reorderDto.ModuleId)
                .ToListAsync();

            if (lessons.Count != reorderDto.OrderedLessonIds.Count)
            {
                return false;
            }

            for (int i = 0; i < reorderDto.OrderedLessonIds.Count; i++)
            {
                var lesson = lessons.FirstOrDefault(l => l.LessonId == reorderDto.OrderedLessonIds[i]);
                if (lesson != null)
                {
                    lesson.OrderNumber = i + 1;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

