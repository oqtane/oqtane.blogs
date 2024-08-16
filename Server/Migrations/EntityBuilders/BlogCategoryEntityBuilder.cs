using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Blogs.Shared;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;
using System;

namespace Oqtane.Blogs.Migrations.EntityBuilders
{
    public class BlogCategoryEntityBuilder : AuditableBaseEntityBuilder<BlogCategoryEntityBuilder>
    {
        private const string _entityTableName = "BlogCategory";
        private readonly PrimaryKey<BlogCategoryEntityBuilder> _primaryKey = new("PK_BlogCategory", x => x.BlogCategoryId);
        private readonly ForeignKey<BlogCategoryEntityBuilder> _blogForeignKey = new("FK_BlogCategory_Blog", x => x.BlogId, "Blog", "BlogId", ReferentialAction.Cascade);
        private readonly ForeignKey<BlogCategoryEntityBuilder> _categorySourceForeignKey = new("FK_BlogCategory_BlogCategorySource", x => x.BlogCategorySourceId, "BlogCategorySource", "BlogCategorySourceId", ReferentialAction.NoAction);

        public BlogCategoryEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_blogForeignKey);
            ForeignKeys.Add(_categorySourceForeignKey);
        }

        protected override BlogCategoryEntityBuilder BuildTable(ColumnsBuilder table)
        {
            BlogCategoryId = AddAutoIncrementColumn(table, "BlogCategoryId");
            BlogId = AddIntegerColumn(table, "BlogId");
            BlogCategorySourceId = AddIntegerColumn(table, "BlogCategorySourceId");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> BlogCategoryId { get; set; }

        public OperationBuilder<AddColumnOperation> BlogId { get; set; }

        public OperationBuilder<AddColumnOperation> BlogCategorySourceId { get; set; }
    }
}
