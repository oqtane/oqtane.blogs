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
    public class BlogTagSourceEntityBuilder : AuditableBaseEntityBuilder<BlogTagSourceEntityBuilder>
    {
        private const string _entityTableName = "BlogTagSource";
        private readonly PrimaryKey<BlogTagSourceEntityBuilder> _primaryKey = new("PK_BlogTagSource", x => x.BlogTagSourceId);
        private readonly ForeignKey<BlogTagSourceEntityBuilder> _moduleForeignKey = new("FK_BlogTagSource_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public BlogTagSourceEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override BlogTagSourceEntityBuilder BuildTable(ColumnsBuilder table)
        {
            BlogTagSourceId = AddAutoIncrementColumn(table, "BlogTagSourceId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            Tag = AddStringColumn(table, "Tag", 50);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> BlogTagSourceId { get; set; }

        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }

        public OperationBuilder<AddColumnOperation> Tag { get; set; }
    }
}
