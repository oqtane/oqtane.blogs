using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Blogs.Models;
using Oqtane.Repository.Databases.Interfaces;

namespace Oqtane.Blogs.Repository
{
    public class BlogContext : DBContextBase, IService, IMultiDatabase
    {
        public virtual DbSet<Blog> Blog { get; set; }

        public BlogContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }
    }
}
