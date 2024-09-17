using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Blogs.Models;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;

namespace Oqtane.Blogs.Services
{
    public interface IBlogTagService
    {
        Task<List<BlogTagSource>> GetBlogTagSourcesAsync(int ModuleId);

    }

    public class BlogTagService : ServiceBase, IBlogTagService, IService
    {
        private readonly SiteState _siteState;

        private string Apiurl => CreateApiUrl("BlogTag", _siteState.Alias);

        public BlogTagService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        public async Task<List<BlogTagSource>> GetBlogTagSourcesAsync(int ModuleId)
        {
            return await GetJsonAsync<List<BlogTagSource>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId));
        }

    }
}
