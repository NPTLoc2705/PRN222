using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.ViewModels
{
    public class CategoryDTO
    {

        public Guid CategoryId { get; set; }
        public string Name { get; set; }
    }
    public class UpdateCategoryDto
    {
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
    }
    public class CategoryDetailsDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public int CourseCount { get; set; }
    }
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
    }
}
