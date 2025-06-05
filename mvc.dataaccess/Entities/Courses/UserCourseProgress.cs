using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.Entities.Courses
{
    public class UserCourseProgress
    {
        public Guid ProgressId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; } // Nullable for course-level progress
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
        public decimal ProgressPercentage { get; set; } = 0.00m;

        // Navigation properties
        public User User { get; set; }
        public Course Course { get; set; }
        public Lesson Lesson { get; set; }
    }
}
