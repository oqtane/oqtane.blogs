using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;
using System;
using Oqtane.Blogs.Shared;

namespace Oqtane.Blogs.Repository
{
    public interface IBlogRepository
    {
        IEnumerable<Blog> GetBlogs(int moduleId, BlogSearch searchQuery);
        Blog GetBlog(int blogId);
        Blog GetBlogBySlug(string slug);
        Blog AddBlog(Blog blog);
        Blog UpdateBlog(Blog blog);
        void DeleteBlog(int blogId);
    }

    public class BlogRepository : IBlogRepository, IService
    {
        private readonly IDbContextFactory<BlogContext> _dbContextFactory;

        public BlogRepository(IDbContextFactory<BlogContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
    }

        public IEnumerable<Blog> GetBlogs(int moduleId, BlogSearch searchQuery)
        {
            var keywords = searchQuery?.Keywords ?? string.Empty;
            var includeDraft = searchQuery?.IncludeDraft ?? false;
            var categories = !string.IsNullOrEmpty(searchQuery?.Categories)
                ? searchQuery.Categories.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt32(i.Trim()))
                : null;
            var pageIndex = searchQuery?.PageIndex ?? 0;
            var pageSize = searchQuery?.PageSize;
            var sortBy = searchQuery?.SortBy ?? string.Empty;
            var sortByDescending = searchQuery?.SortByDescending ?? false;
            var startDate = searchQuery?.StartDate ?? DateTime.MinValue;
            var endDate = searchQuery?.EndDate ?? DateTime.MinValue;
            var tags = !string.IsNullOrEmpty(searchQuery?.Tags) ? searchQuery.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries) : null;
            var excludeBlogs = !string.IsNullOrEmpty(searchQuery?.ExcludeBlogs)
                ? searchQuery.ExcludeBlogs.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt32(i.Trim()))
                : null;

            using var db = _dbContextFactory.CreateDbContext();
            var blogs = db.Blog.AsNoTracking()
                .Include(i => i.BlogContentList)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.BlogCategorySource)
                .Include(i => i.BlogTags)
                .ThenInclude(i => i.BlogTagSource)
                .Select(i => new
                {
                    Blog = i,
                    PublishedVersion = i.BlogContentList.FirstOrDefault(i => i.IsPublished && (i.PublishDate == null || i.PublishDate <= DateTime.UtcNow)),
                    LatestVersion = i.BlogContentList.OrderByDescending(i => i.Version).FirstOrDefault()
                })
                .Where(item =>
                        (moduleId == -1 || item.Blog.ModuleId == moduleId)
                        && (excludeBlogs == null || !excludeBlogs.Any() || !excludeBlogs.Contains(item.Blog.BlogId))
                        && (includeDraft || item.PublishedVersion != null)
                        && (string.IsNullOrEmpty(keywords)
                            || item.Blog.Title.Contains(keywords)
                            || (item.PublishedVersion != null
                                    && item.Blog.BlogContentList.OrderByDescending(i => i.Version).First(i => i.IsPublished && (i.PublishDate == null || i.PublishDate <= DateTime.UtcNow)).Summary.Contains(keywords))
                        )
                        && (categories == null || !categories.Any() || item.Blog.BlogCategories.Any(c => categories.Contains(c.BlogCategorySourceId)))
                        && (tags == null || !tags.Any() || item.Blog.BlogTags.Any(c => tags.Contains(c.BlogTagSource.Tag)))
                        && (startDate == DateTime.MinValue || item.Blog.CreatedOn >= startDate)
                        && (endDate == DateTime.MinValue || item.Blog.CreatedOn <= endDate)
                );

            switch(sortBy.ToLower())
            {
                case "views":
                    blogs = sortByDescending ? blogs.OrderByDescending(i => i.Blog.Views) : blogs.OrderBy(i => i.Blog.Views);
                    break;
                default:
                    blogs = sortByDescending
                        ? blogs.OrderByDescending(i => i.PublishedVersion != null ? i.PublishedVersion.PublishDate : i.LatestVersion.CreatedOn)
                        : blogs.OrderBy(i => i.PublishedVersion != null ? i.PublishedVersion.PublishDate : i.LatestVersion.CreatedOn);
                    break;
            }

            if(pageSize != null && pageSize > 0)
            {
                blogs = blogs.Skip(pageIndex * pageSize.Value).Take(pageSize.Value);
            }

            return blogs.Select(i => i.Blog).ToList();
        }

        public Blog GetBlog(int blogId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.Blog
                .Include(i => i.BlogContentList)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.BlogCategorySource)
                .Include(i => i.BlogTags)
                .ThenInclude(i => i.BlogTagSource)
                .FirstOrDefault(i => i.BlogId == blogId);
        }

        public Blog GetBlogBySlug(string slug)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.Blog
                .Include(i => i.BlogContentList)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.BlogCategorySource)
                .Include(i => i.BlogTags)
                .ThenInclude(i => i.BlogTagSource)
                .FirstOrDefault(i => slug == i.Slug);
        }

        public Blog AddBlog(Blog blog)
        {
            using var db = _dbContextFactory.CreateDbContext();

            //save categories
            blog.BlogCategories = new List<BlogCategory>();
            if(!string.IsNullOrEmpty(blog.Categories))
            {
                foreach (var cat in blog.Categories.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(cat, out int blogCategorySourceId))
                    {
                        blog.BlogCategories.Add(new BlogCategory { BlogCategorySourceId = blogCategorySourceId });
                    }
                }
            }

            //save tags
            blog.BlogTags = new List<BlogTag>();
            if (!string.IsNullOrEmpty(blog.Tags))
            {
                foreach (var tag in blog.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    if(!string.IsNullOrWhiteSpace(tag))
                    {
                        var tagSource = db.BlogTagSource.FirstOrDefault(i => i.Tag == tag);
                        if (tagSource == null)
                        {
                            tagSource = new BlogTagSource { Tag = tag, ModuleId = blog.ModuleId };
                        }

                        blog.BlogTags.Add(new BlogTag { BlogTagSource = tagSource });
                    }
                }
            }

            db.Blog.Add(blog);
            db.SaveChanges();

            return blog;
        }

        public Blog UpdateBlog(Blog blog)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Entry(blog).State = EntityState.Modified;

            //save categories
            var categories = blog.Categories.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt32(i)).ToList();
            foreach (var category in blog.BlogCategories.Where(i => !categories.Any(c => c == i.BlogCategorySourceId)))
            {
                db.Entry(category).State = EntityState.Deleted;
            }
            foreach (var id in categories.Where(i => !blog.BlogCategories.Any(c => c.BlogCategorySourceId == i)))
            {
                blog.BlogCategories.Add(new BlogCategory { BlogCategorySourceId = id });
            }

            //save tags
            var tags = blog.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var tag in blog.BlogTags.Where(i => !tags.Any(c => c.Equals(i.BlogTagSource.Tag, StringComparison.OrdinalIgnoreCase))))
            {
                db.Entry(tag).State = EntityState.Deleted;
            }
            var newTags = tags.Where(i => !blog.BlogTags.Any(c => c.BlogTagSource.Tag.Equals(i, StringComparison.OrdinalIgnoreCase)));
            foreach(var tag in newTags)
            {
                var tagSource = db.BlogTagSource.FirstOrDefault(i => i.Tag == tag);
                if (tagSource == null)
                {
                    tagSource = new BlogTagSource { Tag = tag, ModuleId = blog.ModuleId };
                }
                blog.BlogTags.Add(new BlogTag { BlogTagSource = tagSource });
            }

            db.SaveChanges();
            return blog;
        }

        public void DeleteBlog(int blogId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var Blog = db.Blog.Find(blogId);
            db.Blog.Remove(Blog);
            db.SaveChanges();
        }
    }
}
