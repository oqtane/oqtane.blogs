using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.Blogs.Migrations.EntityBuilders
{
    public class BlogCommentEntityBuilder : AuditableBaseEntityBuilder<BlogCommentEntityBuilder>
    {
        private const string _entityTableName = "BlogComment";
        private readonly PrimaryKey<BlogCommentEntityBuilder> _primaryKey = new("PK_BlogComment", x => x.BlogCommentId);
        private readonly ForeignKey<BlogCommentEntityBuilder> _foreignKey = new("FK_BlogConnent_Blog", x => x.BlogId, "Blog", "BlogId", ReferentialAction.Cascade);

        public BlogCommentEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_foreignKey);
        }

        protected override BlogCommentEntityBuilder BuildTable(ColumnsBuilder table)
        {
            BlogCommentId = AddAutoIncrementColumn(table, "BlogCommentId");
            BlogId = AddIntegerColumn(table, "BlogId");
            Name = AddStringColumn(table, "Name", 50);
            Email = AddStringColumn(table, "Email", 256);
            Comment = AddMaxStringColumn(table, "Comment");
            IsPublished = AddBooleanColumn(table, "IsPublished");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> BlogCommentId { get; set; }

        public OperationBuilder<AddColumnOperation> BlogId { get; set; }

        public OperationBuilder<AddColumnOperation> Name { get; set; }

        public OperationBuilder<AddColumnOperation> Email { get; set; }

        public OperationBuilder<AddColumnOperation> Comment { get; set; }

        public OperationBuilder<AddColumnOperation> IsPublished { get; set; }
    }
}
