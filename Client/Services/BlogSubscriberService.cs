using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Services
{
    public interface IBlogSubscriberService
    {
        Task AddBlogSubscriberAsync(BlogSubscriber BlogSubscriber);
        Task UpdateBlogSubscriberAsync(int ModuleId, string Guid);
        Task DeleteBlogSubscriberAsync(int ModuleId, string Guid);
    }

    public class BlogSubscriberService : ServiceBase, IBlogSubscriberService, IService
    {
        private readonly SiteState _siteState;

        public BlogSubscriberService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        private string Apiurl=> CreateApiUrl("BlogSubscriber", _siteState.Alias);

        public async Task AddBlogSubscriberAsync(BlogSubscriber BlogSubscriber)
        {
            await PostJsonAsync(Apiurl, BlogSubscriber);
        }

        public async Task UpdateBlogSubscriberAsync(int ModuleId, string Guid)
        {
            await PutAsync($"{Apiurl}/{ModuleId}/{Guid}");
        }

        public async Task DeleteBlogSubscriberAsync(int ModuleId, string Guid)
        {
            await DeleteAsync($"{Apiurl}/{ModuleId}/{Guid}");
        }
    }
}
