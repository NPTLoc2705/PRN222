using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.Entities.Courses
{
    public class CourseCategory
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Category name must be between 3 and 100 characters")]
        public string Name { get; set; }

        // Navigation properties
        public ICollection<CourseCategoryMapping> CourseMappings { get; set; } = new List<CourseCategoryMapping>();
    }
}
