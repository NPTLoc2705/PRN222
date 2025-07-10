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
    public class ProgressService : IProgressService
    {
        private readonly IProgressRepo _progressRepo;
        public ProgressService(IProgressRepo progressRepo)
        {
            _progressRepo = progressRepo;
        }

        public Task CreateProgressRecord(UserCourseProgress progress)
        => _progressRepo.CreateProgressRecord(progress);

        public Task DeleteProgressByCourseIdAsync(Guid courseId)
        => _progressRepo.DeleteProgressByCourseIdAsync(courseId);

        public Task<List<Guid>> GetCompletedLessonIds(Guid userId, Guid courseId)
        => _progressRepo.GetCompletedLessonIds(userId, courseId);

        public Task<decimal> GetCourseProgressPercentage(Guid userId, Guid courseId)
        => _progressRepo.GetCourseProgressPercentage(userId, courseId);

        public Task<UserCourseProgress> GetOrCreateProgressRecord(Guid userId, Guid courseId, Guid? lessonId)
        => _progressRepo.GetOrCreateProgressRecord(userId, courseId, lessonId);

        public Task<IEnumerable<UserCourseProgress>> GetUserProgresses(Guid userId)
        => _progressRepo.GetUserProgresses(userId);

        public Task<bool> HasCourseProgress(Guid courseId)
        => _progressRepo.HasCourseProgress(courseId);

        public Task<bool> IsLessonCompleted(Guid userId, Guid courseId, Guid lessonId)
       => _progressRepo.IsLessonCompleted(userId, courseId, lessonId);

        public Task<bool> IsUserEnrolled(Guid userId, Guid courseId)
        => _progressRepo.IsUserEnrolled(userId, courseId);

        public Task UpdateCourseProgressPercentage(Guid userId, Guid courseId)
        => _progressRepo.UpdateCourseProgressPercentage(userId, courseId);

        public Task UpdateLessonProgress(Guid userId, Guid courseId, Guid lessonId, bool isCompleted)
        => _progressRepo.UpdateLessonProgress(userId, courseId, lessonId, isCompleted);
    }
}
