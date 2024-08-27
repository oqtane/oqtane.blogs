using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Blogs.Migrations.EntityBuilders;
using Oqtane.Blogs.Repository;

namespace Oqtane.Blogs.Migrations
{
    [DbContext(typeof(BlogContext))]
    [Migration("Blog.05.02.01.01")]
    public class AddBlogSubscriberIsVerified : MultiDatabaseMigration
    {
        public AddBlogSubscriberIsVerified(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var blogSubscriberEntityBuilder = new BlogSubscriberEntityBuilder(migrationBuilder, ActiveDatabase);
            blogSubscriberEntityBuilder.AddBooleanColumn("IsVerified", true);
            blogSubscriberEntityBuilder.UpdateColumn("IsVerified", "1", "bool", "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var blogSubscriberEntityBuilder = new BlogSubscriberEntityBuilder(migrationBuilder, ActiveDatabase);
            blogSubscriberEntityBuilder.DropColumn("IsVerified");
        }
    }
}
