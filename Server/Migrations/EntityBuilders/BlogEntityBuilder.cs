using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.Blogs.Migrations.EntityBuilders
{
    public class BlogEntityBuilder : AuditableBaseEntityBuilder<BlogEntityBuilder>
    {
        private const string _entityTableName = "Blog";
        private readonly PrimaryKey<BlogEntityBuilder> _primaryKey = new("PK_Blog", x => x.BlogId);
        private readonly ForeignKey<BlogEntityBuilder> _moduleForeignKey = new("FK_Blog_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public BlogEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override BlogEntityBuilder BuildTable(ColumnsBuilder table)
        {
            BlogId = AddAutoIncrementColumn(table, "BlogId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            Title = AddStringColumn(table, "Title", 256, false, true);
            Content = AddMaxStringColumn(table, "Content");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> BlogId { get; set; }

        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }

        public OperationBuilder<AddColumnOperation> Title { get; set; }

        public OperationBuilder<AddColumnOperation> Content { get; set; }
    }
}
