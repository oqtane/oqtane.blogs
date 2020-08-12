using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Services
{
    public class BlogService : ServiceBase, IBlogService, IService
    {
        private readonly SiteState _siteState;

        public BlogService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

         private string Apiurl=> CreateApiUrl(_siteState.Alias, "Blog");

        public async Task<List<Blog>> GetBlogsAsync(int ModuleId)
        {
            List<Blog> Blogs = await GetJsonAsync<List<Blog>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", ModuleId));
            return Blogs.OrderBy(item => item.Title).ToList();
        }

        public async Task<Blog> GetBlogAsync(int BlogId, int ModuleId)
        {
            return await GetJsonAsync<Blog>(CreateAuthorizationPolicyUrl($"{Apiurl}/{BlogId}", ModuleId));
        }

        public async Task<Blog> AddBlogAsync(Blog Blog)
        {
            return await PostJsonAsync<Blog>(CreateAuthorizationPolicyUrl($"{Apiurl}", Blog.ModuleId), Blog);
        }

        public async Task<Blog> UpdateBlogAsync(Blog Blog)
        {
            return await PutJsonAsync<Blog>(CreateAuthorizationPolicyUrl($"{Apiurl}/{Blog.BlogId}", Blog.ModuleId), Blog);
        }

        public async Task DeleteBlogAsync(int BlogId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{BlogId}", ModuleId));
        }
    }
}
