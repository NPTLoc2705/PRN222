using mvc.dataaccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Interfaces
{
    public interface IBlogRepo
    {
        List<Blog> GetAll();
        Blog GetById(int id);
        void Add(Blog blog);
        void Update(Blog blog);
        void Delete(Blog blog);
    }
}
