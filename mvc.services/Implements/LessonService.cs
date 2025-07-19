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
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonService(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<LessonDTO> GetLessonByIdAsync(Guid lessonId)
        {
            return await _lessonRepository.GetLessonByIdAsync(lessonId);
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid moduleId)
        {
            return await _lessonRepository.GetLessonsByCourseIdAsync(moduleId);
        }

        public async Task<LessonDTO> CreateLessonAsync(LessonDTO lessonDto)
        {
            return await _lessonRepository.CreateLessonAsync(lessonDto);
        }

        public async Task<LessonDTO> UpdateLessonAsync(LessonDTO lessonDto)
        {
            return await _lessonRepository.UpdateLessonAsync(lessonDto);
        }

        public async Task<bool> DeleteLessonAsync(Guid lessonId)
        {
            return await _lessonRepository.DeleteLessonAsync(lessonId);
        }

        public async Task<bool> ReorderLessonsAsync(Guid courseId, List<Guid> orderedLessonIds)
        {
            return await _lessonRepository.ReorderLessonsAsync(courseId, orderedLessonIds);
        }

        public Task DeleteLessonsByCourseIdAsync(Guid courseId)
        => _lessonRepository.DeleteLessonsByCourseIdAsync(courseId);
    }
}

