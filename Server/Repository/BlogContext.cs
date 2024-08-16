using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Blogs.Models;
using Oqtane.Repository.Databases.Interfaces;

namespace Oqtane.Blogs.Repository
{
    public class BlogContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<BlogContent> BlogContent { get; set; }
        public virtual DbSet<BlogCategorySource> BlogCategorySource { get; set; }
        public virtual DbSet<BlogCategory> BlogCategory { get; set; }
        public virtual DbSet<BlogTagSource> BlogTagSource { get; set; }
        public virtual DbSet<BlogTag> BlogTag { get; set; }
        public virtual DbSet<BlogSubscriber> BlogSubscriber { get; set; }
        public virtual DbSet<BlogComment> BlogComment { get; set; }

        public BlogContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }
    }
}
