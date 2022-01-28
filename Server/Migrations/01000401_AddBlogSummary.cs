using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Blogs.Migrations.EntityBuilders;
using Oqtane.Blogs.Repository;

namespace Oqtane.Blogs.Migrations
{
    [DbContext(typeof(BlogContext))]
    [Migration("Blog.01.00.04.01")]
    public class AddBlogSummary : MultiDatabaseMigration
    {
        public AddBlogSummary(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var blogEntityBuilder = new BlogEntityBuilder(migrationBuilder, ActiveDatabase);
            blogEntityBuilder.AddStringColumn("Summary", 2000, true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var blogEntityBuilder = new BlogEntityBuilder(migrationBuilder, ActiveDatabase);
            blogEntityBuilder.DropColumn("Summary");
        }
    }
}
