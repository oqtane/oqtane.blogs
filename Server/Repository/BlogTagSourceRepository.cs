using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public interface IBlogTagSourceRepository
    {
        IEnumerable<BlogTagSource> GetBlogTagSources(int moduleId);
        BlogTagSource AddBlogTagSource(BlogTagSource BlogTagSource);
        BlogTagSource UpdateBlogTagSource(BlogTagSource BlogTagSource);
        void DeleteBlogTagSource(int TagSourceId);
    }

    public class BlogTagSourceRepository : IBlogTagSourceRepository, IService
    {
        private readonly IDbContextFactory<BlogContext> _dbContextFactory;

        public BlogTagSourceRepository(IDbContextFactory<BlogContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IEnumerable<BlogTagSource> GetBlogTagSources(int moduleId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.BlogTagSource.AsNoTracking()
                .Where(i => i.ModuleId == moduleId)
                .OrderBy(i => i.Tag)
                .ToList();
        }

        public BlogTagSource AddBlogTagSource(BlogTagSource BlogTagSource)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.BlogTagSource.Add(BlogTagSource);
            db.SaveChanges();

            return BlogTagSource;
        }

        public BlogTagSource UpdateBlogTagSource(BlogTagSource BlogTagSource)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Entry(BlogTagSource).State = EntityState.Modified;
            db.SaveChanges();

            return BlogTagSource;
        }

        public void DeleteBlogTagSource(int TagSourceId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var TagSource = db.BlogTagSource.Find(TagSourceId);
            if (TagSource != null)
            {
                db.BlogTagSource.Remove(TagSource);
                db.SaveChanges();
            }
        }
    }
}
