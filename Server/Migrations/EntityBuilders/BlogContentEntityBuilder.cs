using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;


namespace Oqtane.Blogs.Migrations.EntityBuilders
{
    public class BlogContentEntityBuilder : AuditableBaseEntityBuilder<BlogContentEntityBuilder>
    {
        private const string _entityTableName = "BlogContent";
        private readonly PrimaryKey<BlogContentEntityBuilder> _primaryKey = new("PK_BlogContent", x => x.BlogContentId);
        private readonly ForeignKey<BlogContentEntityBuilder> _blogForeignKey = new("FK_BlogContent_Blog", x => x.BlogId, "Blog", "BlogId", ReferentialAction.Cascade);

        public BlogContentEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_blogForeignKey);
        }

        protected override BlogContentEntityBuilder BuildTable(ColumnsBuilder table)
        {
            BlogContentId = AddAutoIncrementColumn(table, "BlogContentId");
            BlogId = AddIntegerColumn(table, "BlogId");
            Version = AddIntegerColumn(table, "Version");
            Summary = AddStringColumn(table, "Summary", 2000);
            Content = AddMaxStringColumn(table, "Content");
            IsPublished = AddBooleanColumn(table, "IsPublished");
            PublishDate = AddDateTimeColumn(table, "PublishDate", true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> BlogContentId { get; set; }

        public OperationBuilder<AddColumnOperation> BlogId { get; set; }

        public OperationBuilder<AddColumnOperation> Version { get; set; }

        public OperationBuilder<AddColumnOperation> Summary { get; set; }

        public OperationBuilder<AddColumnOperation> Content { get; set; }

        public OperationBuilder<AddColumnOperation> IsPublished { get; set; }

        public OperationBuilder<AddColumnOperation> PublishDate { get; set; }
    }
}
