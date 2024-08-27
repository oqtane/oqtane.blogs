using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;
using Microsoft.EntityFrameworkCore;

namespace Oqtane.Blogs.Repository
{
    public interface IBlogSubscriberRepository
    {
        IEnumerable<BlogSubscriber> GetBlogSubscribers(int ModuleId);
        BlogSubscriber AddBlogSubscriber(BlogSubscriber BlogSubscriber);
        BlogSubscriber UpdateBlogSubscriber(BlogSubscriber BlogSubscriber);
        void DeleteBlogSubscriber(int SubscriberId);
    }

    public class BlogSubscriberRepository : IBlogSubscriberRepository, IService
    {
        private readonly IDbContextFactory<BlogContext> _dbContextFactory;

        public BlogSubscriberRepository(IDbContextFactory<BlogContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IEnumerable<BlogSubscriber> GetBlogSubscribers(int ModuleId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.BlogSubscriber.Where(item => item.ModuleId == ModuleId).ToList();
        }

        public BlogSubscriber AddBlogSubscriber(BlogSubscriber BlogSubscriber)
        {
            using var db = _dbContextFactory.CreateDbContext();
            if (!db.BlogSubscriber.Any(item => item.Email == BlogSubscriber.Email))
            {
                db.BlogSubscriber.Add(BlogSubscriber);
                db.SaveChanges();
                return BlogSubscriber;
            }
            return null;
        }

        public BlogSubscriber UpdateBlogSubscriber(BlogSubscriber BlogSubscriber)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Entry(BlogSubscriber).State = EntityState.Modified;
            db.SaveChanges();
            return BlogSubscriber;
        }

        public void DeleteBlogSubscriber(int BlogSubscriberId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            BlogSubscriber BlogSubscriber = db.BlogSubscriber.Find(BlogSubscriberId);
            db.BlogSubscriber.Remove(BlogSubscriber);
            db.SaveChanges();
        }
    }
}
