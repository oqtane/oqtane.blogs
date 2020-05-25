using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Blogs.Models;
using Oqtane.Blogs.Repository;

namespace Oqtane.Blogs.Manager
{
    public class BlogManager : IInstallable, IPortable
    {
        private IBlogRepository _Blogs;
        private ISqlRepository _sql;

        public BlogManager(IBlogRepository Blogs, ISqlRepository sql)
        {
            _Blogs = Blogs;
            _sql = sql;
        }

        public bool Install(Tenant tenant, string version)
        {
            return _sql.ExecuteScript(tenant, GetType().Assembly, "Oqtane.Blogs." + version + ".sql");
        }

        public bool Uninstall(Tenant tenant)
        {
            return _sql.ExecuteScript(tenant, GetType().Assembly, "Oqtane.Blogs.Uninstall.sql");
        }

        public string ExportModule(Module module)
        {
            string content = "";
            List<Blog> Blogs = _Blogs.GetBlogs(module.ModuleId).ToList();
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
    }
}