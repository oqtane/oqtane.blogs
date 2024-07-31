using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public class TagSourceRepository : ITagSourceRepository, IService
    {
        private readonly IDbContextFactory<BlogContext> _dbContextFactory;

        public TagSourceRepository(IDbContextFactory<BlogContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IEnumerable<TagSource> GetTagSources(int moduleId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.TagSource.AsNoTracking()
                .Where(i => i.ModuleId == moduleId)
                .OrderBy(i => i.Tag)
                .ToList();
        }

        public TagSource AddTagSource(TagSource TagSource)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.TagSource.Add(TagSource);
            db.SaveChanges();

            return TagSource;
        }

        public TagSource UpdateTagSource(TagSource TagSource)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Entry(TagSource).State = EntityState.Modified;
            db.SaveChanges();

            return TagSource;
        }

        public void DeleteTagSource(int TagSourceId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var TagSource = db.TagSource.Find(TagSourceId);
            if (TagSource != null)
            {
                db.TagSource.Remove(TagSource);
                db.SaveChanges();
            }
        }
    }
}
