using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc.dataaccess.Entities;
using mvc.services.Interfaces;

namespace mvc.app.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // GET: Blogs
        public IActionResult Index()
        {
            var blogs = _blogService.GetAll();
            return View(blogs);
        }

        // GET: Blogs/Details/5
        public IActionResult Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = _blogService.GetById(id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,blog_content,ImageData,title")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                // Get User ID from session
                var userIdString = HttpContext.Session.GetString("UserId");

                if (!string.IsNullOrEmpty(userIdString))
                {
                    blog.UserId = Guid.Parse(userIdString);
                    _blogService.Add(blog);
                    return RedirectToAction(nameof(Index));
                }

                // Session expired or user not logged in
                return Unauthorized();
            }

            return View(blog);
        }



        // GET: Blogs/Edit/5
        public IActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var blog = _blogService.GetById(id);

            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,blog_content,ImageData,title")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _blogService.Update(blog);
                }
                catch (Exception ex)
                {
                    // Handle exceptions appropriately
                    // You might want to check if the blog exists first
                    var existingBlog = _blogService.GetById(blog.Id);
                    if (existingBlog == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Convert int to Guid if your service expects Guid
            var guidId = new Guid(); // You'll need to handle the conversion properly
            var blog = _blogService.GetById(guidId);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            // Convert int to Guid if your service expects Guid
                
            var blog = _blogService.GetById(id);

            if (blog != null)
            {
                _blogService.Delete(blog);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}