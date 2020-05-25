using System.Collections.Generic;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public interface IBlogRepository
    {
        IEnumerable<Blog> GetBlogs(int ModuleId);
        Blog GetBlog(int BlogId);
        Blog AddBlog(Blog Blog);
        Blog UpdateBlog(Blog Blog);
        void DeleteBlog(int BlogId);
    }
}
