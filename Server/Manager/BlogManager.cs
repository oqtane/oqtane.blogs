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

namespace Oqtane.Blogs.Manager
{
    public class BlogManager : MigratableModuleBase, IInstallable, IPortable, ISitemap
    {
        private IBlogRepository _Blogs;
        private ISqlRepository _sql;
		private readonly IDBContextDependencies _DBContextDependencies;

		public BlogManager(IBlogRepository Blogs, ISqlRepository sql, IDBContextDependencies DBContextDependencies)
        {
            _Blogs = Blogs;
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
            return Migrate(new BlogContext(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new BlogContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Module module)
        {
            string content = "";
            List<Blog> Blogs = _Blogs.GetBlogs(module.ModuleId, "").ToList();
            if (Blogs != null)
            {
                content = JsonSerializer.Serialize(Blogs);
            }
            return content;
        }

        public void ImportModule(Module module, string content, string version)
        {
            List<Blog> Blogs = null;
            if (!string.IsNullOrEmpty(content))
            {
                Blogs = JsonSerializer.Deserialize<List<Blog>>(content);
            }
            if (Blogs != null)
            {
                foreach(Blog Blog in Blogs)
                {
                    Blog _Blog = new Blog();
                    _Blog.ModuleId = module.ModuleId;
                    _Blog.Title = Blog.Title;
                    _Blog.Content = Blog.Content;
                    _Blogs.AddBlog(_Blog);
                }
            }
        }

        public List<Sitemap> GetUrls(string alias, string path, Module module)
        {
            var sitemap = new List<Sitemap>();
            List<Blog> Blogs = _Blogs.GetBlogs(module.ModuleId, "").ToList();
            foreach (var Blog in Blogs.Where(item => item.Published))
            {
                var parameters = Utilities.AddUrlParameters(Blog.BlogId, Common.FormatSlug(Blog.Title));
                sitemap.Add(new Sitemap { Url = Utilities.NavigateUrl(alias, path, parameters), ModifiedOn = Blog.ModifiedOn });
            }
            return sitemap;
        }
    }
}