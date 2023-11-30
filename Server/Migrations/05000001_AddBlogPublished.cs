using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Blogs.Migrations.EntityBuilders;
using Oqtane.Blogs.Repository;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.Blogs.Migrations
{
    [DbContext(typeof(BlogContext))]
    [Migration("Blog.05.00.00.01")]
    public class AddBlogPublished : MultiDatabaseMigration
    {
        public AddBlogPublished(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var blogEntityBuilder = new BlogEntityBuilder(migrationBuilder, ActiveDatabase);
            blogEntityBuilder.AddBooleanColumn("Published", true);
            blogEntityBuilder.UpdateColumn("Published", "1", "bool", "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var blogEntityBuilder = new BlogEntityBuilder(migrationBuilder, ActiveDatabase);
            blogEntityBuilder.DropColumn("Published");
        }
    }
}
