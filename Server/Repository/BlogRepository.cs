using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public class BlogRepository : IBlogRepository, IService
    {
        private readonly BlogContext _db;

        public BlogRepository(BlogContext context)
        {
            _db = context;
        }

        public IEnumerable<Blog> GetBlogs(int ModuleId, string Search)
        {
            return _db.Blog.Where(item => item.ModuleId == ModuleId &&
                (string.IsNullOrEmpty(Search) || item.Title.Contains(Search) || item.Summary.Contains(Search)))
                .OrderByDescending(item => item.CreatedOn);
        }

        public Blog GetBlog(int BlogId)
        {
            return _db.Blog.Find(BlogId);
        }

        public Blog AddBlog(Blog Blog)
        {
            _db.Blog.Add(Blog);
            _db.SaveChanges();
            return Blog;
        }

        public Blog UpdateBlog(Blog Blog)
        {
            _db.Entry(Blog).State = EntityState.Modified;
            _db.SaveChanges();
            return Blog;
        }

        public void DeleteBlog(int BlogId)
        {
            Blog Blog = _db.Blog.Find(BlogId);
            _db.Blog.Remove(Blog);
            _db.SaveChanges();
        }
    }
}
