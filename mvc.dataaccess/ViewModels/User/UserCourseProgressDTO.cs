using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.ViewModels.User
{
    public class UserCourseProgressDTO
    {
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }   
        public Guid? LessonId { get; set; }
        public string LessonTitle { get; set; }   
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime LastAccessed { get; set; }
        public decimal ProgressPercentage { get; set; }
    }
}
