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
    public class ProgressRepo : IProgressRepo
    {
        private readonly AppDbContext _context;
        public ProgressRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetCourseProgressPercentage(Guid userId, Guid courseId)
        {
            var progress = await _context.UserCourseProgresses
                 .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId && p.LessonId == null);
            return progress?.ProgressPercentage ?? 0;
        }

        public async Task<UserCourseProgress> GetOrCreateProgressRecord(Guid userId, Guid courseId, Guid? lessonId)
        {
            var progress = await _context.UserCourseProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId && p.LessonId == lessonId);

            if (progress == null)
            {
                progress = new UserCourseProgress
                {
                    UserId = userId,
                    CourseId = courseId,
                    LessonId = lessonId,
                    IsCompleted = false,
                    ProgressPercentage = 0.00m,
                    LastAccessed = DateTime.UtcNow
                };
                _context.UserCourseProgresses.Add(progress);
                await _context.SaveChangesAsync();
            }
            return progress;
        }

        public async Task<IEnumerable<UserCourseProgress>> GetUserProgresses(Guid userId)
        {
            return await _context.UserCourseProgresses
              .Where(p => p.UserId == userId)
              .Include(p => p.Course)
              .Include(p => p.Lesson)
              .ToListAsync();
        }

        public async Task UpdateCourseProgressPercentage(Guid userId, Guid courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null) return;

            var totalLessons = course.Lessons.Count;
            if (totalLessons == 0) return;

            var completedLessons = await _context.UserCourseProgresses
                .Where(p => p.UserId == userId &&
                           p.CourseId == courseId &&
                           p.LessonId != null &&
                           p.IsCompleted)
                .CountAsync();

            var progressRecord = await GetOrCreateProgressRecord(userId, courseId, null);
            var progressPercentage = (decimal)completedLessons / totalLessons * 100;
            progressRecord.ProgressPercentage = Math.Round(progressPercentage, 2);
            progressRecord.LastAccessed = DateTime.UtcNow;

            _context.UserCourseProgresses.Update(progressRecord);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLessonProgress(Guid userId, Guid courseId, Guid lessonId, bool isCompleted)
        {
            var progress = await GetOrCreateProgressRecord(userId, courseId, lessonId);
            progress.IsCompleted = isCompleted;
            progress.LastAccessed = DateTime.UtcNow;

            if (isCompleted)
            {
                progress.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                progress.CompletedAt = null;
            }

            _context.UserCourseProgresses.Update(progress);
            await _context.SaveChangesAsync();

            // Update overall course progress
            await UpdateCourseProgressPercentage(userId, courseId);
        }

        public async Task<bool> IsLessonCompleted(Guid userId, Guid courseId, Guid lessonId)
        {
            return await _context.UserCourseProgresses
                .AnyAsync(p => p.UserId == userId &&
                              p.CourseId == courseId &&
                              p.LessonId == lessonId &&
                              p.IsCompleted);
        }

        public async Task<List<Guid>> GetCompletedLessonIds(Guid userId, Guid courseId)
        {
            return await _context.UserCourseProgresses
                .Where(p => p.UserId == userId &&
                           p.CourseId == courseId &&
                           p.LessonId != null &&
                           p.IsCompleted)
                .Select(p => p.LessonId.Value)
                .ToListAsync();
        }

        public async Task<bool> IsUserEnrolled(Guid userId, Guid courseId)
        {
            return await _context.UserCourseProgresses
                .AnyAsync(p => p.UserId == userId && p.CourseId == courseId && p.LessonId == null);
        }

        public async Task CreateProgressRecord(UserCourseProgress progress)
        {
            // Check if record already exists
            var existing = await _context.UserCourseProgresses
                .FirstOrDefaultAsync(p => p.UserId == progress.UserId &&
                                         p.CourseId == progress.CourseId &&
                                         p.LessonId == null);

            if (existing == null)
            {
                _context.UserCourseProgresses.Add(progress);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasCourseProgress(Guid courseId)
        {
            return await _context.UserCourseProgresses
                .AnyAsync(p => p.CourseId == courseId);
        }

        public async Task DeleteProgressByCourseIdAsync(Guid courseId)
        {
            var progressRecords = await _context.UserCourseProgresses
                .Where(p => p.CourseId == courseId)
                .ToListAsync();

            if (progressRecords.Any())
            {
                _context.UserCourseProgresses.RemoveRange(progressRecords);
                await _context.SaveChangesAsync();
            }
        }
    }
}