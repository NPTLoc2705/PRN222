    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    namespace mvc.dataaccess.Entities.Courses
    {
        public class Course
        {
            public Guid CourseId { get; set; } = Guid.NewGuid();
            public string Title { get; set; }
            public string Description { get; set; }
            public int Duration { get; set; } // in minutes
            public string DifficultyLevel { get; set; }
            public byte[] ImageBytes { get; set; } // Binary image data (optional)
            public string ImageContentType { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
            public bool IsActive { get; set; } = true;

            // Navigation properties
            public ICollection<Module> Modules { get; set; }
            public ICollection<UserCourseProgress> UserProgresses { get; set; }
            public ICollection<CourseCategoryMapping> CategoryMappings { get; set; }
            public ICollection<CoursePrerequisite> Prerequisites { get; set; }
            public ICollection<CoursePrerequisite> RequiredForCourses { get; set; }
        }
    }
