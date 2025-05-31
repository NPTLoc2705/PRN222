using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid StaffId { get; set; }
        public virtual User Staff { get; set; } // Navigation property to User
        public bool IsPublished { get; set; } = false;
        // Additional properties can be added as needed
    }
}
