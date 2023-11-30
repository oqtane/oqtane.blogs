using System.Threading.Tasks;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Services
{
    public interface ISubscriberService 
    {
        Task AddSubscriberAsync(Subscriber Subscriber);

        Task DeleteSubscriberAsync(int ModuleId, string Guid);
    }
}
