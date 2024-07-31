using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Blogs.Models;
using Oqtane.Blogs.Repository;
using Oqtane.Shared;
using Oqtane.Migrations.Framework;
using Oqtane.Enums;
using Oqtane.Blogs.Shared;
using Oqtane.Interfaces;
using System;
using System.Threading.Tasks;

namespace Oqtane.Blogs.Manager
{
    public class BlogManager : MigratableModuleBase, IInstallable, IPortable, ISitemap, ISearchable
    {
        private IBlogRepository _blogRepository;
        private IAliasRepository _aliasRepository;
        private IPageRepository _pageRepository;
        private ISqlRepository _sql;
		private readonly IDBContextDependencies _DBContextDependencies;

		public BlogManager(IBlogRepository blogRepository, IAliasRepository aliasRepository, IPageRepository pageRepository, ISqlRepository sql, IDBContextDependencies DBContextDependencies)
        {
            _blogRepository = blogRepository;
            _aliasRepository = aliasRepository;
            _pageRepository = pageRepository;
            _sql = sql;
            _DBContextDependencies = DBContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            if (tenant.DBType == Constants.DefaultDBType && version == "1.0.3")
            {
                // version 1.0.0 used SQL scripts rather than migrations, so we need to seed the migration history table
                _sql.ExecuteNonQuery(tenant, MigrationUtils.BuildInsertScript("Blog.01.00.00.00"));
            }

            var migrated = Migrate(new BlogContext(_DBContextDependencies), tenant, MigrationType.Up);

            if (migrated && version == "5.1.0")
            {
                UpdateBlogSlug(tenant);
            }

            return migrated;
        }

        private void UpdateBlogSlug(Tenant tenant)
        {
            var blogs = _blogRepository.GetBlogs(-1, new BlogSearch { IncludeDraft = true });
            foreach (var blog in blogs)
            {
                blog.Slug = Common.FormatSlug(blog.Title);
                _blogRepository.UpdateBlog(blog);
            }
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new BlogContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Module module)
        {
            string content = "";
            List<Blog> blogs = _blogRepository.GetBlogs(module.ModuleId, new BlogSearch { IncludeDraft = true }).ToList();
            if (blogs != null)
            {
                content = JsonSerializer.Serialize(blogs);
            }
            return content;
        }

        public void ImportModule(Module module, string content, string version)
        {
            List<Blog> blogs = null;
            if (!string.IsNullOrEmpty(content))
            {
                blogs = JsonSerializer.Deserialize<List<Blog>>(content);
            }
            if (blogs != null)
            {
                foreach(Blog Blog in blogs)
                {
                    var blog = new Blog();
                    blog.ModuleId = module.ModuleId;
                    blog.Title = Blog.Title;
                    blog = _blogRepository.AddBlog(blog);
                    
                }
            }
        }

        public List<Sitemap> GetUrls(string alias, string path, Module module)
        {
            var sitemap = new List<Sitemap>();
            var blogs = _blogRepository.GetBlogs(module.ModuleId, null).ToList();
            foreach (var blog in blogs.Where(item => item.PublishedBlogContent != null))
            {
                var parameters = Utilities.AddUrlParameters(blog.BlogId, Common.FormatSlug(blog.Title));
                sitemap.Add(new Sitemap { Url = Utilities.NavigateUrl(alias, path, parameters), ModifiedOn = blog.ModifiedOn });
            }
            return sitemap;
        }

        public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexOn)
        {
            var searchContents = new List<SearchContent>();

            var blogs = _blogRepository.GetBlogs(pageModule.ModuleId, null).Where(i => i.ModifiedOn >= lastIndexOn);
            foreach (var blog in blogs)
            {
                var blogContent = blog.PublishedBlogContent;
                var searchContent = new SearchContent
                {
                    EntityName = "Blog",
                    EntityId = blog.BlogId.ToString(),
                    Title = blog.Title,
                    Description = blogContent.Summary,
                    Body = blogContent.Content,
                    Url = BlogUtilities.FormatUrl("", pageModule.Page.Path, blog),
                    ContentModifiedBy = blogContent.ModifiedBy,
                    ContentModifiedOn = blogContent.ModifiedOn
                };

                searchContents.Add(searchContent);
            }

            return Task.FromResult(searchContents);
        }
    }
}