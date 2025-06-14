using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.ViewModels
{
    public class ModuleDTO
    {
        public Guid ModuleId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, int.MaxValue)]
        public int OrderNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    // Separate class for error responses
    public class ModuleErrorResponse : ModuleDTO
    {
        public bool Error { get; set; }
        public string? Message { get; set; }
    }
}
