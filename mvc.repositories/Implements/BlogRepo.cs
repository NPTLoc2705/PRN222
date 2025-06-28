using mvc.dataaccess.Entities;
using mvc.repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Implements
{
    public class BlogRepo : IBlogRepo
    {
        private readonly AppDbContext _context;

        public BlogRepo(AppDbContext context)
        {
            _context = context;
        }

        public List<Blog> GetAll() => _context.Blogs.ToList();

        public Blog GetById(Guid id) => _context.Blogs.Find(id);

        public void Add(Blog blog)
        {
            _context.Blogs.Add(blog);
            _context.SaveChanges();
        }

        public void Update(Blog blog)
        {
            _context.Blogs.Update(blog);
            _context.SaveChanges();
        }

        public void Delete(Blog blog)
        {
            _context.Blogs.Remove(blog);
            _context.SaveChanges();
        }
    }
}
