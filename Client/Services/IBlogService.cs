using System.Collections.Generic;
using System.Threading.Tasks;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Services
{
    public interface IBlogService 
    {
        Task<List<Blog>> GetBlogsAsync(int ModuleId, string Search);

        Task<Blog> GetBlogAsync(int BlogId, int ModuleId);

        Task<Blog> AddBlogAsync(Blog Blog);

        Task<Blog> UpdateBlogAsync(Blog Blog);

        Task DeleteBlogAsync(int BlogId, int ModuleId);

        Task NotifyAsync(int BlogId, int ModuleId);
    }
}
