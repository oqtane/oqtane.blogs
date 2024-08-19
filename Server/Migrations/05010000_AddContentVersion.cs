using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Blogs.Migrations.EntityBuilders;
using Oqtane.Blogs.Repository;

namespace Oqtane.Blogs.Migrations
{
    [DbContext(typeof(BlogContext))]
    [Migration("Blog.05.01.00.00")]
    public class AddContentVersion : MultiDatabaseMigration
    {
        public AddContentVersion(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var blogContentEntityBuilder = new BlogContentEntityBuilder(migrationBuilder, ActiveDatabase);
            blogContentEntityBuilder.Create();

            //merge content
            migrationBuilder.Sql("INSERT INTO [BlogContent] SELECT BlogId, 1, Summary, Content, Published, NULL, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn FROM [Blog]");

            var blogEntityBuilder = new BlogEntityBuilder(migrationBuilder, ActiveDatabase);
            blogEntityBuilder.DropColumn("Published");
            blogEntityBuilder.DropColumn("Summary");
            blogEntityBuilder.DropColumn("Content");
            blogEntityBuilder.AddStringColumn("Slug", 255, true);
            blogEntityBuilder.AddIntegerColumn("Thumbnail", false, 0);
            blogEntityBuilder.AddStringColumn("AlternateText", 200, true);
            blogEntityBuilder.AddIntegerColumn("Views", false, 0);

            var blogCategorySourceEntityBuilder = new BlogCategorySourceEntityBuilder(migrationBuilder, ActiveDatabase);
            blogCategorySourceEntityBuilder.Create();

            var blogCategoryEntityBuilder = new BlogCategoryEntityBuilder(migrationBuilder, ActiveDatabase);
            blogCategoryEntityBuilder.Create();

            var blogTagSourceEntityBuilder = new BlogTagSourceEntityBuilder(migrationBuilder, ActiveDatabase);
            blogTagSourceEntityBuilder.Create();

            var blogTagEntityBuilder = new BlogTagEntityBuilder(migrationBuilder, ActiveDatabase);
            blogTagEntityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var blogEntityBuilder = new BlogEntityBuilder(migrationBuilder, ActiveDatabase);
            blogEntityBuilder.AddBooleanColumn("Published", true);
            blogEntityBuilder.AddStringColumn("Summary", 2000);
            blogEntityBuilder.AddMaxStringColumn("Content");
            blogEntityBuilder.UpdateColumn("Published", "1", "bool", "");

            //merge content
            migrationBuilder.Sql(@"
UPDATE b SET b.Summary = c.Summary, b.Content = c.Content, b.Published = c.IsPublished
FROM [Blog] AS b INNER JOIN [BlogContent] as c ON c.BlogId = b.BlogId
WHERE c.BlogContentId IN (SELECT MAX(BlogContentId) FROM [BlogContent] GROUP BY BlogId)");

            var blogContentEntityBuilder = new BlogContentEntityBuilder(migrationBuilder, ActiveDatabase);
            blogContentEntityBuilder.Drop();

            var blogCategoryEntityBuilder = new BlogCategoryEntityBuilder(migrationBuilder, ActiveDatabase);
            blogCategoryEntityBuilder.Drop();

            var blogCategorySourceEntityBuilder = new BlogCategorySourceEntityBuilder(migrationBuilder, ActiveDatabase);
            blogCategorySourceEntityBuilder.Drop();

            var blogTagEntityBuilder = new BlogTagEntityBuilder(migrationBuilder, ActiveDatabase);
            blogTagEntityBuilder.Drop();

            var blogTagSourceEntityBuilder = new BlogTagSourceEntityBuilder(migrationBuilder, ActiveDatabase);
            blogTagSourceEntityBuilder.Drop();
        }
    }
}
