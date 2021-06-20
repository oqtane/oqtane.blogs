using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;
using System.Threading.Tasks;

namespace Oqtane.Blogs.Repository
{
    public class BlogRepository : IBlogRepository, IService
    {
        private readonly BlogContext _db;

        public BlogRepository(BlogContext context)
        {
            _db = context;
        }

        public IEnumerable<Blog> GetBlogs(int ModuleId)
        {
            return _db.Blog.Where(item => item.ModuleId == ModuleId);
        }

        public async Task<IEnumerable<Blog>> GetBlogsAsync(int ModuleId, bool tracking = true)
        {
            if (tracking)
            {
                return await _db.Blog.Where(item => item.ModuleId == ModuleId).ToListAsync();
            }
            return await _db.Blog.AsNoTracking().Where(item => item.ModuleId == ModuleId).ToListAsync();
        }

        public async Task<Blog> GetBlogAsync(int BlogId)
        {
            return await _db.Blog.FindAsync(BlogId).AsTask();
        }

        public Blog AddBlog(Blog Blog)
        {
            _db.Blog.Add(Blog);
            _db.SaveChanges();
            return Blog;
        }

        public async Task<Blog> AddBlogAsync(Blog Blog)
        {
            _db.Blog.Add(Blog);
            await _db.SaveChangesAsync();
            return Blog;
        }

        public async Task<Blog> UpdateBlogAsync(Blog Blog)
        {
            _db.Entry(Blog).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Blog;
        }

        public void DeleteBlog(int BlogId)
        {
            Blog Blog = _db.Blog.Find(BlogId);
            _db.Blog.Remove(Blog);
            _db.SaveChanges();
        }

        public async Task DeleteBlogAsync(int BlogId)
        {
            Blog Blog = await _db.Blog.FindAsync(BlogId);
            _db.Blog.Remove(Blog);
            await _db.SaveChangesAsync();
        }
    }
}
