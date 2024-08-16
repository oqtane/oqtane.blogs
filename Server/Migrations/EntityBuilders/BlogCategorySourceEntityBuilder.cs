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
    public class BlogCategorySourceEntityBuilder : AuditableBaseEntityBuilder<BlogCategorySourceEntityBuilder>
    {
        private const string _entityTableName = "BlogCategorySource";
        private readonly PrimaryKey<BlogCategorySourceEntityBuilder> _primaryKey = new("PK_BlogCategorySource", x => x.BlogCategorySourceId);
        private readonly ForeignKey<BlogCategorySourceEntityBuilder> _moduleForeignKey = new("FK_BlogCategorySource_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public BlogCategorySourceEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override BlogCategorySourceEntityBuilder BuildTable(ColumnsBuilder table)
        {
            BlogCategorySourceId = AddAutoIncrementColumn(table, "BlogCategorySourceId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            Name = AddStringColumn(table, "Name", 50);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> BlogCategorySourceId { get; set; }

        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }

        public OperationBuilder<AddColumnOperation> Name { get; set; }
    }
}
