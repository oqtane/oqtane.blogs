using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Blogs.Models;
using Oqtane.Infrastructure;
using Oqtane.Repository.Databases.Interfaces;

namespace Oqtane.Blogs.Repository
{
    public class BlogContext : DBContextBase, IService, IMultiDatabase
    {
        public virtual DbSet<Blog> Blog { get; set; }

        public BlogContext(ITenantManager tenantManager, IHttpContextAccessor accessor) : base(tenantManager, accessor)
        {
            // ContextBase handles multi-tenant database connections
        }
    }
}
