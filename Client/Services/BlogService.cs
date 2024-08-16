using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using Oqtane.Blogs.Models;
using System.Web;

namespace Oqtane.Blogs.Services
{
    public interface IBlogService
    {
        Task<List<Blog>> GetBlogsAsync(int moduleId, BlogSearch searchQuery);

        Task<Blog> GetBlogAsync(int blogId, int moduleId);

        Task<Blog> GetBlogBySlugAsync(string slug, int moduleId);

        Task<Blog> AddBlogAsync(Blog blog);

        Task<Blog> UpdateBlogAsync(Blog blog);

        Task UpdateBlogViewsAsync(int blogId, int moduleId);

        Task DeleteBlogAsync(int blogId, int moduleId);

        Task<int> NotifyAsync(int blogId, int moduleId);
    }

    public class BlogService : ServiceBase, IBlogService, IService
    {
        private readonly SiteState _siteState;

        public BlogService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

         private string Apiurl=> CreateApiUrl("Blog", _siteState.Alias);

        public async Task<List<Blog>> GetBlogsAsync(int moduleId, BlogSearch searchQuery)
        {
            var query = $"?moduleid={moduleId}";
            if (searchQuery != null)
            {
                var properties = from p in searchQuery.GetType().GetProperties()
                                 where p.GetValue(searchQuery, null) != null
                                 select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(searchQuery, null).ToString());

                query += "&" + string.Join("&", properties.ToArray());
            }

            return await GetJsonAsync<List<Blog>>(CreateAuthorizationPolicyUrl($"{Apiurl}{query}", EntityNames.Module, moduleId));
        }

        public async Task<Blog> GetBlogAsync(int blogId, int moduleId)
        {
            return await GetJsonAsync<Blog>(CreateAuthorizationPolicyUrl($"{Apiurl}/{blogId}", EntityNames.Module, moduleId));
        }

        public async Task<Blog> GetBlogBySlugAsync(string slug, int moduleId)
        {
            return await GetJsonAsync<Blog>(CreateAuthorizationPolicyUrl($"{Apiurl}/slug/{slug}", EntityNames.Module, moduleId));
        }

        public async Task<Blog> AddBlogAsync(Blog blog)
        {
            return await PostJsonAsync<Blog>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, blog.ModuleId), blog);
        }

        public async Task<Blog> UpdateBlogAsync(Blog blog)
        {
            return await PutJsonAsync<Blog>(CreateAuthorizationPolicyUrl($"{Apiurl}/{blog.BlogId}", EntityNames.Module, blog.ModuleId), blog);
        }

        public async Task UpdateBlogViewsAsync(int blogId, int moduleId)
        {
            await PutAsync(CreateAuthorizationPolicyUrl($"{Apiurl}?id={blogId}", EntityNames.Module, moduleId));
        }

        public async Task DeleteBlogAsync(int blogId, int moduleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{blogId}", EntityNames.Module, moduleId));
        }

        public async Task<int> NotifyAsync(int blogId, int moduleId)
        {
            return await GetJsonAsync<int>(CreateAuthorizationPolicyUrl($"{Apiurl}/notify/{blogId}", EntityNames.Module, moduleId));
        }
    }
}
