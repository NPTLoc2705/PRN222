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

        public async Task<IEnumerable<LessonDTO>> GetLessonsByModuleIdAsync(Guid moduleId)
        {
            return await _lessonRepository.GetLessonsByModuleIdAsync(moduleId);
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

        public async Task<bool> ReorderLessonsAsync(LessonDTO reorderDto)
        {
            return await _lessonRepository.ReorderLessonsAsync(reorderDto);
        }
    }
}

