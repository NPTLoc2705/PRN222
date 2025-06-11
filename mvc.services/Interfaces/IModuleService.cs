using mvc.dataaccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface IModuleService
    {
        Task<ModuleDTO> GetModuleByIdAsync(Guid moduleId);
        Task<IEnumerable<ModuleDTO>> GetModulesByCourseIdAsync(Guid courseId);
        Task<ModuleDTO> CreateModuleAsync(ModuleDTO moduleDto);
        Task<ModuleDTO> UpdateModuleAsync(ModuleDTO moduleDto);
        Task<bool> DeleteModuleAsync(Guid moduleId, Guid courseId);
    }
}
