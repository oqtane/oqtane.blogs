using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace Oqtane.Blogs.Repository
{
    public class CategorySourceRepository : ICategorySourceRepository, IService
    {
        private const string CategorySourcesCacheKey = "blogcategorysources_{0}";

        private readonly IDbContextFactory<BlogContext> _dbContextFactory;
        private readonly IMemoryCache _cache;

        public CategorySourceRepository(IDbContextFactory<BlogContext> dbContextFactory, IMemoryCache cache)
        {
            _dbContextFactory = dbContextFactory;
            _cache = cache;
        }

        public IEnumerable<CategorySource> GetCategorySources(int moduleId)
        {
            var cacheKey = string.Format(CategorySourcesCacheKey, moduleId);
            return _cache.GetOrCreate<IEnumerable<CategorySource>>(cacheKey, entry =>
            {
                using var db = _dbContextFactory.CreateDbContext();
                var blogIds = from b in db.BlogContent
                              where b.PublishStatus == Shared.PublishStatus.Published || (b.PublishStatus == Shared.PublishStatus.Scheduled && b.PublishDate <= DateTime.UtcNow)
                              select b.BlogId;
                var data = (from c in db.CategorySource
                            join bc in db.Category on c.CategorySourceId equals bc.CategorySourceId
                            where blogIds.Contains(bc.BlogId) && c.ModuleId == moduleId
                            group new { c, bc } by c.CategorySourceId into g
                            select new { CategorySourceId = g.Key, Items = g.ToList() }
                           ).ToList();

                return data.Select(i =>
                {
                    var categorySource = i.Items[0].c;
                    categorySource.BlogCount = i.Items.DistinctBy(i => i.bc.BlogId).Count();

                    return categorySource;
                }).OrderBy(i => i.Name);
            });
        }

        public CategorySource AddCategorySource(CategorySource categorySource)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.CategorySource.Add(categorySource);
            db.SaveChanges();

            ClearCache(categorySource.ModuleId);
            return categorySource;
        }

        public CategorySource UpdateCategorySource(CategorySource categorySource)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Entry(categorySource).State = EntityState.Modified;
            db.SaveChanges();

            ClearCache(categorySource.ModuleId);
            return categorySource;
        }

        public void DeleteCategorySource(int categorySourceId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var categorySource = db.CategorySource.Find(categorySourceId);
            if (categorySource != null)
            {
                db.CategorySource.Remove(categorySource);

                ClearCache(categorySource.ModuleId);
                db.SaveChanges();
            }
        }

        private void ClearCache(int moduleId)
        {
            var cacheKey = string.Format(CategorySourcesCacheKey, moduleId);
            _cache.Remove(cacheKey);
        }
    }
}
