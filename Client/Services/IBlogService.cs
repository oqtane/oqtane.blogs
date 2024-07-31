using System.Collections.Generic;
using System.Threading.Tasks;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Services
{
    public interface IBlogService 
    {
        Task<List<Blog>> GetBlogsAsync(int moduleId, BlogSearch searchQuery);

        Task<Blog> GetBlogAsync(int blogId, int moduleId);

        Task<Blog> GetBlogBySlugAsync(string slug, int moduleId);

        Task<Blog> AddBlogAsync(Blog blog);

        Task<Blog> UpdateBlogAsync(Blog blog);

        Task DeleteBlogAsync(int blogId, int moduleId);

        Task<int> NotifyAsync(int blogId, int moduleId);
    }
}
