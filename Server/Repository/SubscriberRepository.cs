using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;
using Microsoft.EntityFrameworkCore;

namespace Oqtane.Blogs.Repository
{
    public class SubscriberRepository : ISubscriberRepository, IService
    {
        private readonly IDbContextFactory<BlogContext> _dbContextFactory;

        public SubscriberRepository(IDbContextFactory<BlogContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IEnumerable<Subscriber> GetSubscribers(int ModuleId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.Subscriber.Where(item => item.ModuleId == ModuleId).ToList();
        }

        public Subscriber AddSubscriber(Subscriber Subscriber)
        {
            using var db = _dbContextFactory.CreateDbContext();
            if (!db.Subscriber.Any(item => item.Email == Subscriber.Email))
            {
                db.Subscriber.Add(Subscriber);
                db.SaveChanges();
                return Subscriber;
            }
            return null;
        }

        public void DeleteSubscriber(int SubscriberId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            Subscriber Subscriber = db.Subscriber.Find(SubscriberId);
            db.Subscriber.Remove(Subscriber);
            db.SaveChanges();
        }
    }
}
