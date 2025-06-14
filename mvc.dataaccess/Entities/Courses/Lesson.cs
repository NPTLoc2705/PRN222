using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.Entities.Courses
{
    public class Lesson
    {
        public Guid LessonId { get; set; } = Guid.NewGuid();
        public Guid ModuleId { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; } // Video, Article, Quiz, etc.
        public string ContentUrl { get; set; }
        public int Duration { get; set; } // in minutes
        public int OrderNumber { get; set; }
        public bool IsFreePreview { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Module Module { get; set; }
        public ICollection<UserCourseProgress> UserProgresses { get; set; }
    }
}
