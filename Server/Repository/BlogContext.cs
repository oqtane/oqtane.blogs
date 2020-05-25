using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public class BlogContext : DBContextBase, IService
    {
        public virtual DbSet<Blog> Blog { get; set; }

        public BlogContext(ITenantResolver tenantResolver, IHttpContextAccessor accessor) : base(tenantResolver, accessor)
        {
            // ContextBase handles multi-tenant database connections
        }
    }
}
