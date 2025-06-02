using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.Entities
{
    public class Blog
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string blog_content { get; set; }
        public byte[] ImageData { get; set; }
        public string title { get; set; }
    }
}
