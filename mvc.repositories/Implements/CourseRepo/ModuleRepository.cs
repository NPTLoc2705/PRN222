using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.Entities.Courses;
using mvc.dataaccess.ViewModels;
using mvc.repositories.Interfaces.ICourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Implements.CourseRepo
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly AppDbContext _context;
        private readonly ICourseRepository _courseRepository;

        public ModuleRepository(AppDbContext context, ICourseRepository courseRepository)
        {
            _context = context;
            _courseRepository = courseRepository;
        }
        public async Task<ModuleDTO> GetByIdAsync(Guid moduleId)
        {
            var module = await _context.Modules
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId);

            return module == null ? null : MapToDTO(module);
        }

        public async Task<IEnumerable<ModuleDTO>> GetByCourseIdAsync(Guid courseId)
        {
            var modules = await _context.Modules
                .AsNoTracking()
                .Where(m => m.CourseId == courseId)
                .OrderBy(m => m.OrderNumber)
                .ToListAsync();

            return modules.Select(MapToDTO);
        }

        public async Task<ModuleDTO> CreateAsync(ModuleDTO moduleDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!await _courseRepository.CourseExistsAsync(moduleDto.CourseId))
                {
                    throw new ArgumentException("Specified course does not exist", nameof(moduleDto.CourseId));
                }

                var module = new Module
                {
                    CourseId = moduleDto.CourseId,
                    Title = moduleDto.Title,
                    Description = moduleDto.Description,
                    OrderNumber = moduleDto.OrderNumber,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Modules.Add(module);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return MapToDTO(module);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<ModuleDTO> UpdateAsync(ModuleDTO moduleDto)
        {
            // Verify module exists AND belongs to specified course
            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.ModuleId == moduleDto.ModuleId &&
                                        m.CourseId == moduleDto.CourseId);

            if (module == null)
            {
                throw new KeyNotFoundException("Module not found or doesn't belong to specified course");
            }

            module.Title = moduleDto.Title;
            module.Description = moduleDto.Description;
            module.OrderNumber = moduleDto.OrderNumber;

            await _context.SaveChangesAsync();
            return MapToDTO(module);
        }

        public async Task<bool> DeleteAsync(Guid moduleId, Guid courseId)
        {
            // Verify the module belongs to the course
            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId
                                       && m.CourseId == courseId);

            if (module == null) return false;

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }


        private static ModuleDTO MapToDTO(Module module)
        {
            return new ModuleDTO
            {
                ModuleId = module.ModuleId,
                CourseId = module.CourseId,
                Title = module.Title,
                Description = module.Description,
                OrderNumber = module.OrderNumber,
                CreatedAt = module.CreatedAt
            };
        }
    }
}

