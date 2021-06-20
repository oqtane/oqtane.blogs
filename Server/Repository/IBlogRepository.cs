using System.Collections.Generic;
using System.Threading.Tasks;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public interface IBlogRepository
    {
        IEnumerable<Blog> GetBlogs(int ModuleId);
        Task<IEnumerable<Blog>> GetBlogsAsync(int ModuleId, bool tracking = true);
        Task<Blog> GetBlogAsync(int BlogId);
        Blog AddBlog(Blog Blog);
        Task<Blog> AddBlogAsync(Blog Blog);
        Task<Blog> UpdateBlogAsync(Blog Blog);
        void DeleteBlog(int BlogId);
        Task DeleteBlogAsync(int BlogId);
    }
}
