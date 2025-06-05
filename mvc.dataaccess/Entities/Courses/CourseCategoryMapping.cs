using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.Entities.Courses
{
    public class CourseCategoryMapping
    {
        public Guid CourseId { get; set; }
        public Guid CategoryId { get; set; }

        // Navigation properties
        public Course Course { get; set; }
        public CourseCategory Category { get; set; }
    }
}
