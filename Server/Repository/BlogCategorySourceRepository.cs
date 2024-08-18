using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace Oqtane.Blogs.Repository
{
    public interface IBlogCategorySourceRepository
    {
        IEnumerable<BlogCategorySource> GetBlogCategorySources(int moduleId);
        BlogCategorySource AddBlogCategorySource(BlogCategorySource blogCategorySource);
        BlogCategorySource UpdateBlogCategorySource(BlogCategorySource blogCategorySource);
        void DeleteBlogCategorySource(int categorySourceId);
    }

    public class BlogCategorySourceRepository : IBlogCategorySourceRepository, IService
    {
        private const string BlogCategorySourcesCacheKey = "blogcategorysources_{0}";

        private readonly IDbContextFactory<BlogContext> _dbContextFactory;
        private readonly IMemoryCache _cache;

        public BlogCategorySourceRepository(IDbContextFactory<BlogContext> dbContextFactory, IMemoryCache cache)
        {
            _dbContextFactory = dbContextFactory;
            _cache = cache;
        }

        public IEnumerable<BlogCategorySource> GetBlogCategorySources(int moduleId)
        {
            var cacheKey = string.Format(BlogCategorySourcesCacheKey, moduleId);
            return _cache.GetOrCreate<IEnumerable<BlogCategorySource>>(cacheKey, entry =>
            {
                using var db = _dbContextFactory.CreateDbContext();
                var blogIds = from b in db.BlogContent
                              where b.IsPublished && (b.PublishDate == null || b.PublishDate <= DateTime.UtcNow)
                              select b.BlogId;
                var data = (from c in db.BlogCategorySource
                            from bc in db.BlogCategory.Where(i => i.BlogCategorySourceId == c.BlogCategorySourceId).DefaultIfEmpty()
                            where (bc == null || blogIds.Contains(bc.BlogId)) && c.ModuleId == moduleId
                            group new { c, bc } by c.BlogCategorySourceId into g
                            select new { CategorySourceId = g.Key, Items = g.ToList() }
                           ).ToList();

                return data.Select(i =>
                {
                    var categorySource = i.Items[0].c;
                    categorySource.BlogCount = i.Items.Any(item => item.bc == null) ? 0 : i.Items.DistinctBy(i => i.bc.BlogId).Count();

                    return categorySource;
                }).OrderBy(i => i.Name);
            });
        }

        public BlogCategorySource AddBlogCategorySource(BlogCategorySource blogCategorySource)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.BlogCategorySource.Add(blogCategorySource);
            db.SaveChanges();

            ClearCache(blogCategorySource.ModuleId);
            return blogCategorySource;
        }

        public BlogCategorySource UpdateBlogCategorySource(BlogCategorySource blogCategorySource)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Entry(blogCategorySource).State = EntityState.Modified;
            db.SaveChanges();

            ClearCache(blogCategorySource.ModuleId);
            return blogCategorySource;
        }

        public void DeleteBlogCategorySource(int categorySourceId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var categorySource = db.BlogCategorySource.Find(categorySourceId);
            if (categorySource != null)
            {
                db.BlogCategorySource.Remove(categorySource);

                ClearCache(categorySource.ModuleId);
                db.SaveChanges();
            }
        }

        private void ClearCache(int moduleId)
        {
            var cacheKey = string.Format(BlogCategorySourcesCacheKey, moduleId);
            _cache.Remove(cacheKey);
        }
    }
}
