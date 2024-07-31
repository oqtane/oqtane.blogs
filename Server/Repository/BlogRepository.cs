using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;
using System;
using Oqtane.Blogs.Shared;

namespace Oqtane.Blogs.Repository
{
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
                .ThenInclude(i => i.CategorySource)
                .Include(i => i.BlogTags)
                .ThenInclude(i => i.TagSource)
                .Where(item =>
                        (moduleId == -1 || item.ModuleId == moduleId)
                        && (excludeBlogs == null || !excludeBlogs.Any() || !excludeBlogs.Contains(item.BlogId))
                        && (includeDraft || item.BlogContentList.Any(i => i.PublishStatus == PublishStatus.Published || (i.PublishStatus == PublishStatus.Scheduled && i.PublishDate <= DateTime.UtcNow)))
                        && (string.IsNullOrEmpty(keywords)
                            || item.Title.Contains(keywords)
                            || (item.BlogContentList.Any(i => i.PublishStatus == PublishStatus.Published || (i.PublishStatus == PublishStatus.Scheduled && i.PublishDate <= DateTime.UtcNow))
                                    && item.BlogContentList.OrderByDescending(i => i.Version).First(i => i.PublishStatus == PublishStatus.Published || (i.PublishStatus == PublishStatus.Scheduled && i.PublishDate <= DateTime.UtcNow)).Summary.Contains(keywords))
                        )
                        && (categories == null || !categories.Any() || item.BlogCategories.Any(c => categories.Contains(c.CategorySourceId)))
                        && (tags == null || !tags.Any() || item.BlogTags.Any(c => tags.Contains(c.TagSource.Tag)))
                        && (startDate == DateTime.MinValue || item.CreatedOn >= startDate)
                        && (endDate == DateTime.MinValue || item.CreatedOn <= endDate)
                );

            switch(sortBy.ToLower())
            {
                case "views":
                    blogs = sortByDescending ? blogs.OrderByDescending(i => i.Views) : blogs.OrderBy(i => i.Views);
                    break;
                default:
                    blogs = blogs.OrderByDescending(i => i.BlogId);
                    break;
            }

            if(pageSize != null && pageSize > 0)
            {
                blogs = blogs.Skip(pageIndex * pageSize.Value).Take(pageSize.Value);
            }

            return blogs.ToList();
        }

        public Blog GetBlog(int blogId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.Blog
                .Include(i => i.BlogContentList)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.CategorySource)
                .Include(i => i.BlogTags)
                .ThenInclude(i => i.TagSource)
                .FirstOrDefault(i => i.BlogId == blogId);
        }

        public Blog GetBlogBySlug(string slug)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.Blog
                .Include(i => i.BlogContentList)
                .Include(i => i.BlogCategories)
                .ThenInclude(i => i.CategorySource)
                .Include(i => i.BlogTags)
                .ThenInclude(i => i.TagSource)
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
                    if (int.TryParse(cat, out int categorySourceId))
                    {
                        blog.BlogCategories.Add(new BlogCategory { CategorySourceId = categorySourceId });
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
                        var tagSource = db.TagSource.FirstOrDefault(i => i.Tag == tag);
                        if (tagSource == null)
                        {
                            tagSource = new TagSource { Tag = tag, ModuleId = blog.ModuleId };
                        }

                        blog.BlogTags.Add(new BlogTag { TagSource = tagSource });
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
            if (!string.IsNullOrEmpty(blog.Categories))
            {
                var categories = blog.Categories.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt32(i)).ToList();

                //mark deleted items
                foreach (var c in blog.BlogCategories.Where(i => !categories.Any(c => c == i.CategorySourceId)))
                {
                    db.Entry(c).State = EntityState.Deleted;
                }

                //add new items
                foreach (var id in categories.Where(i => !blog.BlogCategories.Any(c => c.CategorySourceId == i)))
                {
                    blog.BlogCategories.Add(new BlogCategory { CategorySourceId = id });
                }
            }

            //save tags
            if (!string.IsNullOrEmpty(blog.Tags))
            {
                var tags = blog.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var deletedTags = blog.BlogTags.Where(i => !tags.Any(c => c.Equals(i.TagSource.Tag, StringComparison.OrdinalIgnoreCase)));
                foreach (var tag in deletedTags)
                {
                    blog.BlogTags.Remove(tag);
                }

                var newTags = tags.Where(i => !blog.BlogTags.Any(c => c.TagSource.Tag.Equals(i, StringComparison.OrdinalIgnoreCase)));
                foreach(var tag in newTags)
                {
                    var tagSource = db.TagSource.FirstOrDefault(i => i.Tag == tag);
                    if (tagSource == null)
                    {
                        tagSource = new TagSource { Tag = tag, ModuleId = blog.ModuleId };
                    }

                    blog.BlogTags.Add(new BlogTag { TagSource = tagSource });
                }
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

        public IEnumerable<BlogContent> GetBlogContents(int blogId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.BlogContent.AsNoTracking()
                .Where(i => i.BlogId == blogId)
                .OrderByDescending(i => i.Version)
                .ToList();
        }

        public BlogContent AddBlogContent(BlogContent blogContent)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.BlogContent.Add(blogContent);
            db.SaveChanges();
            return blogContent;
        }

        public BlogContent UpdateBlogContent(BlogContent blogContent)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Entry(blogContent).State = EntityState.Modified;
            db.SaveChanges();
            return blogContent;
        }

        public BlogContent RestoreBlogContent(BlogContent blogContent)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var latestVersion = GetBlogContents(blogContent.BlogId).OrderByDescending(i => i.Version).FirstOrDefault();
            if(latestVersion != null && latestVersion.BlogContentId != blogContent.BlogContentId)
            {
                if(latestVersion.PublishStatus == Shared.PublishStatus.Draft 
                    || (latestVersion.PublishStatus == Shared.PublishStatus.Scheduled && latestVersion.PublishDate >= DateTime.UtcNow))
                {
                    latestVersion.Summary = blogContent.Summary;
                    latestVersion.Content = blogContent.Content;

                    db.Entry(latestVersion).State = EntityState.Modified;
                    db.SaveChanges();

                    return latestVersion;
                }
                else
                {
                    var newVersion = new BlogContent
                    {
                        BlogId = blogContent.BlogId,
                        Version = latestVersion.Version + 1,
                        Summary = blogContent.Summary,
                        Content = blogContent.Content,
                        PublishStatus = Shared.PublishStatus.Draft,
                        PublishDate = null
                    };
                    db.BlogContent.Add(newVersion);
                    db.SaveChanges();

                    return newVersion;
                }
            }

            return null;
        }

        public void DeleteBlogContent(int blogContentId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var blogContent = db.BlogContent.Find(blogContentId);
            if (blogContent != null)
            {
                db.BlogContent.Remove(blogContent);
                db.SaveChanges();
            }
        }
    }
}
