using mvc.dataaccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Interfaces.ICourse
{
    public interface IModuleRepository
    {
        Task<ModuleDTO> GetByIdAsync(Guid moduleId);
        Task<IEnumerable<ModuleDTO>> GetByCourseIdAsync(Guid courseId);
        Task<ModuleDTO> CreateAsync(ModuleDTO moduleDto);
        Task<ModuleDTO> UpdateAsync(ModuleDTO moduleDto);
        Task<bool> DeleteAsync(Guid moduleId, Guid courseId);
    }
}
