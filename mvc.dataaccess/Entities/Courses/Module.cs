using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.Entities.Courses
{
    public class Module
    {
        public Guid ModuleId { get; set; } = Guid.NewGuid();
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Course Course { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
    }
}
