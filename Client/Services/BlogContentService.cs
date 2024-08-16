using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
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

    public class BlogContentService : ServiceBase, IBlogContentService, IService
    {
        private readonly SiteState _siteState;

        public BlogContentService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

         private string Apiurl=> CreateApiUrl("BlogContent", _siteState.Alias);

        public async Task<List<BlogContent>> GetBlogContentAsync(int moduleId, int blogId)
        {
            return await GetJsonAsync<List<BlogContent>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}&blogId={blogId}", EntityNames.Module, moduleId));
        }

        public async Task<BlogContent> AddBlogContentAsync(int moduleId, BlogContent blogContent)
        {
            return await PostJsonAsync<BlogContent>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}&type=add", EntityNames.Module, moduleId), blogContent);
        }

        public async Task<BlogContent> UpdateBlogContentAsync(int moduleId, BlogContent blogContent)
        {
            return await PutJsonAsync<BlogContent>(CreateAuthorizationPolicyUrl($"{Apiurl}/{blogContent.BlogContentId}?moduleId={moduleId}", EntityNames.Module, moduleId), blogContent);
        }

        public async Task<BlogContent> RestoreBlogContentAsync(int moduleId, BlogContent blogContent)
        {
            return await PostJsonAsync<BlogContent>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}&type=restore", EntityNames.Module, moduleId), blogContent);
        }

        public async Task DeleteBlogContentAsync(int moduleId, int blogId, int blogContentId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{blogId}/{blogContentId}?moduleId={moduleId}", EntityNames.Module, moduleId));
        }
    }
}
