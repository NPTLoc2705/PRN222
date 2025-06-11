using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.Entities.Courses
{
    public class CourseCategory
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public ICollection<CourseCategoryMapping> CourseMappings { get; set; }
    }
}
