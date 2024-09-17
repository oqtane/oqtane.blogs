using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Blogs.Models;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;

namespace Oqtane.Blogs.Services
{
    public interface IBlogCategoryService
    {
        Task<List<BlogCategorySource>> GetBlogCategorySourcesAsync(int ModuleId);

        Task<BlogCategorySource> AddBlogCategorySourceAsync(BlogCategorySource blogCategorySource);

        Task<BlogCategorySource> UpdateBlogCategorySourceAsync(BlogCategorySource blogCategorySource);

        Task DeleteBlogCategorySourceAsync(int categorySourceId, int ModuleId);
    }

    public class BlogCategoryService : ServiceBase, IBlogCategoryService, IService
    {
        private readonly SiteState _siteState;

        private string Apiurl => CreateApiUrl("BlogCategory", _siteState.Alias);

        public BlogCategoryService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        public async Task<List<BlogCategorySource>> GetBlogCategorySourcesAsync(int ModuleId)
        {
            return await GetJsonAsync<List<BlogCategorySource>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<BlogCategorySource> AddBlogCategorySourceAsync(BlogCategorySource blogCategorySource)
        {
            return await PostJsonAsync<BlogCategorySource>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, blogCategorySource.ModuleId), blogCategorySource);
        }

        public async Task<BlogCategorySource> UpdateBlogCategorySourceAsync(BlogCategorySource blogCategorySource)
        {
            return await PutJsonAsync<BlogCategorySource>(CreateAuthorizationPolicyUrl($"{Apiurl}/{blogCategorySource.BlogCategorySourceId}", EntityNames.Module, blogCategorySource.ModuleId), blogCategorySource);
        }

        public async Task DeleteBlogCategorySourceAsync(int categorySourceId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{categorySourceId}", EntityNames.Module, ModuleId));
        }
    }
}
