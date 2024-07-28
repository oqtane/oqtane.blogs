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
    public class CategoryService : ServiceBase, ICategoryService, IService
    {
        private readonly SiteState _siteState;

        private string Apiurl => CreateApiUrl("Category", _siteState.Alias);

        public CategoryService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        public async Task<List<CategorySource>> GetCategorySourcesAsync(int ModuleId)
        {
            return await GetJsonAsync<List<CategorySource>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<CategorySource> AddCategorySourceAsync(CategorySource categorySource)
        {
            return await PostJsonAsync<CategorySource>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, categorySource.ModuleId), categorySource);
        }

        public async Task<CategorySource> UpdateCategorySourceAsync(CategorySource categorySource)
        {
            return await PutJsonAsync<CategorySource>(CreateAuthorizationPolicyUrl($"{Apiurl}/{categorySource.CategorySourceId}", EntityNames.Module, categorySource.ModuleId), categorySource);
        }

        public async Task DeleteCategorySourceAsync(int categorySourceId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{categorySourceId}", EntityNames.Module, ModuleId));
        }
    }
}
