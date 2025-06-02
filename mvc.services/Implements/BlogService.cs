using mvc.dataaccess.Entities;
using mvc.repositories.Interfaces;
using mvc.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Implements
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepo _repository;

        public BlogService(IBlogRepo repository)
        {
            _repository = repository;
        }

        public List<Blog> GetAll() => _repository.GetAll();

        public Blog GetById(int id) => _repository.GetById(id);

        public void Add(Blog blog) => _repository.Add(blog);

        public void Update(Blog blog) => _repository.Update(blog);

        public void Delete(Blog blog) => _repository.Delete(blog);
    }
}
