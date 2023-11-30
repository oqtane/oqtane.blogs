using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Blogs.Migrations.EntityBuilders;
using Oqtane.Blogs.Repository;

namespace Oqtane.Blogs.Migrations
{
    [DbContext(typeof(BlogContext))]
    [Migration("Blog.05.00.00.00")]
    public class AddSubscribers : MultiDatabaseMigration
    {
        public AddSubscribers(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new SubscriberEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new SubscriberEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Drop();
        }
    }
}
