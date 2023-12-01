using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public class SubscriberRepository : ISubscriberRepository, IService
    {
        private readonly BlogContext _db;

        public SubscriberRepository(BlogContext context)
        {
            _db = context;
        }

        public IEnumerable<Subscriber> GetSubscribers(int ModuleId)
        {
            return _db.Subscriber.Where(item => item.ModuleId == ModuleId);
        }

        public Subscriber AddSubscriber(Subscriber Subscriber)
        {
            if (!_db.Subscriber.Any(item => item.Email == Subscriber.Email))
            {
                _db.Subscriber.Add(Subscriber);
                _db.SaveChanges();
                return Subscriber;
            }
            return null;
        }

        public void DeleteSubscriber(int SubscriberId)
        {
            Subscriber Subscriber = _db.Subscriber.Find(SubscriberId);
            _db.Subscriber.Remove(Subscriber);
            _db.SaveChanges();
        }
    }
}
