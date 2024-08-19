using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;
using System;

namespace Oqtane.Blogs.Repository
{
    public interface IBlogContentRepository
    {
        IEnumerable<BlogContent> GetBlogContents(int blogId);
        BlogContent AddBlogContent(BlogContent blogContent);
        BlogContent UpdateBlogContent(BlogContent blogContent);
        BlogContent RestoreBlogContent(BlogContent blogContent);
        void DeleteBlogContent(int blogContentId);
    }

    public class BlogContentRepository : IBlogContentRepository, IService
    {
        private readonly IDbContextFactory<BlogContext> _dbContextFactory;

        public BlogContentRepository(IDbContextFactory<BlogContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
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
                if(!latestVersion.IsPublished || latestVersion.PublishDate > DateTime.UtcNow)
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
                        IsPublished = false,
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
