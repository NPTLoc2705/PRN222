using mvc.dataaccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface IBlogService
    {
        List<Blog> GetAll();
        Blog GetById(Guid id);
        void Add(Blog blog);
        void Update(Blog blog);
        void Delete(Blog blog);
    }
}
