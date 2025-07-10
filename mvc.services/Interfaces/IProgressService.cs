using mvc.dataaccess.Entities.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface IProgressService
    {
        Task<UserCourseProgress> GetOrCreateProgressRecord(Guid userId, Guid courseId, Guid? lessonId);
        Task UpdateLessonProgress(Guid userId, Guid courseId, Guid lessonId, bool isCompleted);
        Task UpdateCourseProgressPercentage(Guid userId, Guid courseId);
        Task<decimal> GetCourseProgressPercentage(Guid userId, Guid courseId);
        Task<IEnumerable<UserCourseProgress>> GetUserProgresses(Guid userId);

        Task<List<Guid>> GetCompletedLessonIds(Guid userId, Guid courseId);
        Task<bool> IsLessonCompleted(Guid userId, Guid courseId, Guid lessonId);
        Task<bool> HasCourseProgress(Guid courseId);
        Task DeleteProgressByCourseIdAsync(Guid courseId);

        Task CreateProgressRecord(UserCourseProgress progress);
        Task<bool> IsUserEnrolled(Guid userId, Guid courseId);
    }
}
