using mvc.dataaccess.Entities.Courses;
using mvc.dataaccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface ILessonService
    {
        Task<LessonDTO> GetLessonByIdAsync(Guid lessonId);
        Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId); 
        Task<LessonDTO> CreateLessonAsync(LessonDTO lessonDto);
        Task<LessonDTO> UpdateLessonAsync(LessonDTO lessonDto);
        Task<bool> DeleteLessonAsync(Guid lessonId);
        Task<bool> ReorderLessonsAsync(Guid courseId, List<Guid> orderedLessonIds);
        Task DeleteLessonsByCourseIdAsync(Guid courseId);
    }
}
