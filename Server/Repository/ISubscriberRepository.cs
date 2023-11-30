using System.Collections.Generic;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public interface ISubscriberRepository
    {
        IEnumerable<Subscriber> GetSubscribers(int ModuleId);
        Subscriber AddSubscriber(Subscriber Subscriber);
        void DeleteSubscriber(int SubscriberId);
    }
}
