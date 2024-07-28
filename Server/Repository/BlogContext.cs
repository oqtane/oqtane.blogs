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
        public virtual DbSet<CategorySource> CategorySource { get; set; }
        public virtual DbSet<BlogCategory> Category { get; set; }
        public virtual DbSet<TagSource> TagSource { get; set; }
        public virtual DbSet<BlogTag> Tag { get; set; }
        public virtual DbSet<Subscriber> Subscriber { get; set; }

        public BlogContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }
    }
}
