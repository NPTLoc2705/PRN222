using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.Entities.Courses
{
    public class CoursePrerequisite
    {
        public Guid CourseId { get; set; }
        public Guid PrerequisiteCourseId { get; set; }

        // Navigation properties
        public Course Course { get; set; }
        public Course PrerequisiteCourse { get; set; }
    }
}
