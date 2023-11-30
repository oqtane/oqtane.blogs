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
    public class SubscriberService : ServiceBase, ISubscriberService, IService
    {
        private readonly SiteState _siteState;

        public SubscriberService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        private string Apiurl=> CreateApiUrl("Subscriber", _siteState.Alias);

        public async Task AddSubscriberAsync(Subscriber Subscriber)
        {
            await PostJsonAsync(Apiurl, Subscriber);
        }

        public async Task DeleteSubscriberAsync(int ModuleId, string Guid)
        {
            await DeleteAsync($"{Apiurl}/{ModuleId}/{Guid}");
        }
    }
}
