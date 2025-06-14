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
        Task<IEnumerable<LessonDTO>> GetLessonsByModuleIdAsync(Guid moduleId);
        Task<LessonDTO> CreateLessonAsync(LessonDTO lessonDto);
        Task<LessonDTO> UpdateLessonAsync(LessonDTO lessonDto);
        Task<bool> DeleteLessonAsync(Guid lessonId);
        Task<bool> ReorderLessonsAsync(LessonDTO reorderDto);
    }
}
