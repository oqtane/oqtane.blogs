using System.Collections.Generic;
using System.Threading.Tasks;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Services
{
    public interface IBlogContentService
    {
        Task<List<BlogContent>> GetBlogContentAsync(int moduleId, int blogId);

        Task<BlogContent> AddBlogContentAsync(int moduleId, BlogContent blogContent);

        Task<BlogContent> UpdateBlogContentAsync(int moduleId, BlogContent blogContent);

        Task<BlogContent> RestoreBlogContentAsync(int moduleId, BlogContent blogContent);

        Task DeleteBlogContentAsync(int moduleId, int blogId, int blogContentId);
    }
}
