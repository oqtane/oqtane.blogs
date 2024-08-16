using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Blogs.Migrations.EntityBuilders;
using Oqtane.Blogs.Repository;

namespace Oqtane.Blogs.Migrations
{
    [DbContext(typeof(BlogContext))]
    [Migration("Blog.05.02.00.01")]
    public class AddBlogComment : MultiDatabaseMigration
    {
        public AddBlogComment(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var blogCommentEntityBuilder = new BlogCommentEntityBuilder(migrationBuilder, ActiveDatabase);
            blogCommentEntityBuilder.Create();

            var blogEntityBuilder = new BlogEntityBuilder(migrationBuilder, ActiveDatabase);
            blogEntityBuilder.AddBooleanColumn("AllowComments", true);
            blogEntityBuilder.UpdateColumn("AllowComments", "1", "bool", "");

            var blogSubscriberEntityBuilder = new BlogSubscriberEntityBuilder(migrationBuilder, ActiveDatabase);
            blogSubscriberEntityBuilder.Create();
            migrationBuilder.Sql("INSERT INTO [BlogSubscriber] SELECT ModuleId, Email, Guid, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn FROM [Subscriber]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // not implemented
        }
    }
}
