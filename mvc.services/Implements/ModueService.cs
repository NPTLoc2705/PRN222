using mvc.dataaccess.ViewModels;
using mvc.repositories.Interfaces.ICourse;
using mvc.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mvc.services.Implementations
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ICourseRepository _courseRepository;

        public ModuleService(IModuleRepository moduleRepository, ICourseRepository courseRepository)
        {
            _moduleRepository = moduleRepository;
            _courseRepository = courseRepository;
        }

        public async Task<ModuleDTO> GetModuleByIdAsync(Guid moduleId)
        {
            return await _moduleRepository.GetByIdAsync(moduleId);
        }

        public async Task<IEnumerable<ModuleDTO>> GetModulesByCourseIdAsync(Guid courseId)
        {
            return await _moduleRepository.GetByCourseIdAsync(courseId);
        }

        public async Task<ModuleDTO> CreateModuleAsync(ModuleDTO moduleDto)
        {
            // Add validation
            if (string.IsNullOrWhiteSpace(moduleDto.Title))
                throw new ArgumentException("Title is required");

            if (moduleDto.OrderNumber <= 0)
                throw new ArgumentException("Order number must be positive");

            return await _moduleRepository.CreateAsync(moduleDto);
        }

        public async Task<ModuleDTO> UpdateModuleAsync(ModuleDTO moduleDto)
        {
            // Verify the module belongs to the course first
            var existingModule = await _moduleRepository.GetByIdAsync(moduleDto.ModuleId);
            if (existingModule == null || existingModule.CourseId != moduleDto.CourseId)
            {
                throw new InvalidOperationException("Module not found or course mismatch");
            }

            return await _moduleRepository.UpdateAsync(moduleDto);
        }

        public async Task<bool> DeleteModuleAsync(Guid moduleId, Guid courseId)
        {
            // Verify the module belongs to the course first
            var existingModule = await _moduleRepository.GetByIdAsync(moduleId);
            if (existingModule == null || existingModule.CourseId != courseId)
            {
                return false;
            }

            return await _moduleRepository.DeleteAsync(moduleId, courseId);
        }
    }
}