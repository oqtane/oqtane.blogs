using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Services
{
    public interface IBlogCommentService
    {
        Task<List<BlogComment>> GetBlogCommentsAsync(int blogId, int moduleId);

        Task<BlogComment> GetBlogCommentAsync(int blogCommentId, int blogId, int moduleId);

        Task<BlogComment> AddBlogCommentAsync(BlogComment blogComment, int moduleId);

        Task<BlogComment> UpdateBlogCommentAsync(BlogComment blogComment, int moduleId);

        Task DeleteBlogCommentAsync(int blogCommentId, int blogId, int moduleId);
    }

    public class BlogCommentService : ServiceBase, IBlogCommentService, IService
    {
        private readonly SiteState _siteState;

        public BlogCommentService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

         private string Apiurl => CreateApiUrl("BlogComment", _siteState.Alias);

        public async Task<List<BlogComment>> GetBlogCommentsAsync(int blogId, int moduleId)
        {
            return await GetJsonAsync<List<BlogComment>>(CreateAuthorizationPolicyUrl($"{Apiurl}?id={blogId}", EntityNames.Module, moduleId));
        }

        public async Task<BlogComment> GetBlogCommentAsync(int blogCommentId, int blogId, int moduleId)
        {
            return await GetJsonAsync<BlogComment>(CreateAuthorizationPolicyUrl($"{Apiurl}/{blogCommentId}/{blogId}", EntityNames.Module, moduleId));
        }

        public async Task<BlogComment> AddBlogCommentAsync(BlogComment blogComment, int moduleId)
        {
            return await PostJsonAsync<BlogComment>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, moduleId), blogComment);
        }

        public async Task<BlogComment> UpdateBlogCommentAsync(BlogComment blogComment, int moduleId)
        {
            return await PutJsonAsync<BlogComment>(CreateAuthorizationPolicyUrl($"{Apiurl}/{blogComment.BlogCommentId}", EntityNames.Module, moduleId), blogComment);
        }

        public async Task DeleteBlogCommentAsync(int blogCommentId, int blogId, int moduleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{blogCommentId}/{blogId}", EntityNames.Module, moduleId));
        }
    }
}
