using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;
using System;

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

            var blogIds = from b in db.BlogContent
                            where b.IsPublished && (b.PublishDate == null || b.PublishDate <= DateTime.UtcNow)
                            select b.BlogId;

            var data = (from c in db.BlogTagSource
                        from bc in db.BlogTag.Where(i => i.BlogTagSourceId == c.BlogTagSourceId).DefaultIfEmpty()
                        where (bc == null || blogIds.Contains(bc.BlogId)) && c.ModuleId == moduleId
                        group new { c, bc } by c.BlogTagSourceId into g
                        select new { TagSourceId = g.Key, Items = g.ToList() }
                        ).ToList();

            return data.Select(i =>
            {
                var tagSource = i.Items[0].c;
                tagSource.BlogCount = i.Items.Any(item => item.bc == null) ? 0 : i.Items.DistinctBy(i => i.bc.BlogId).Count();

                return tagSource;
            }).OrderBy(i => i.Tag);

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
