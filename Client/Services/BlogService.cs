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
            List<Blog> Blogs = await GetJsonAsync<List<Blog>>($"{Apiurl}?moduleid={ModuleId}");
            return Blogs.OrderBy(item => item.Title).ToList();
        }

        public async Task<Blog> GetBlogAsync(int BlogId)
        {
            return await GetJsonAsync<Blog>($"{Apiurl}/{BlogId}");
        }

        public async Task<Blog> AddBlogAsync(Blog Blog)
        {
            return await PostJsonAsync<Blog>($"{Apiurl}?entityid={Blog.ModuleId}", Blog);
        }

        public async Task<Blog> UpdateBlogAsync(Blog Blog)
        {
            return await PutJsonAsync<Blog>($"{Apiurl}/{Blog.BlogId}?entityid={Blog.ModuleId}", Blog);
        }

        public async Task DeleteBlogAsync(int BlogId)
        {
            await DeleteAsync($"{Apiurl}/{BlogId}");
        }
    }
}
