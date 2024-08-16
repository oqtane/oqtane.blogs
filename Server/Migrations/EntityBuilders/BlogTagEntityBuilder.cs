using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.Blogs.Migrations.EntityBuilders
{
    public class BlogTagEntityBuilder : AuditableBaseEntityBuilder<BlogTagEntityBuilder>
    {
        private const string _entityTableName = "BlogTag";
        private readonly PrimaryKey<BlogTagEntityBuilder> _primaryKey = new("PK_BlogTag", x => x.BlogTagId);
        private readonly ForeignKey<BlogTagEntityBuilder> _blogForeignKey = new("FK_BlogTag_Blog", x => x.BlogId, "Blog", "BlogId", ReferentialAction.Cascade);
        private readonly ForeignKey<BlogTagEntityBuilder> _tagSourceForeignKey = new("FK_BlogTag_BlogTagSource", x => x.BlogTagSourceId, "BlogTagSource", "BlogTagSourceId", ReferentialAction.NoAction);

        public BlogTagEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_blogForeignKey);
            ForeignKeys.Add(_tagSourceForeignKey);
        }

        protected override BlogTagEntityBuilder BuildTable(ColumnsBuilder table)
        {
            BlogTagId = AddAutoIncrementColumn(table, "BlogTagId");
            BlogId = AddIntegerColumn(table, "BlogId");
            BlogTagSourceId = AddIntegerColumn(table, "BlogTagSourceId");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> BlogTagId { get; set; }

        public OperationBuilder<AddColumnOperation> BlogId { get; set; }

        public OperationBuilder<AddColumnOperation> BlogTagSourceId { get; set; }
    }
}
