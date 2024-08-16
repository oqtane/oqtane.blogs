using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.Blogs.Migrations.EntityBuilders
{
    public class BlogSubscriberEntityBuilder : AuditableBaseEntityBuilder<BlogSubscriberEntityBuilder>
    {
        private const string _entityTableName = "BlogSubscriber";
        private readonly PrimaryKey<BlogSubscriberEntityBuilder> _primaryKey = new("PK_BlogSubscriber", x => x.BlogSubscriberId);
        private readonly ForeignKey<BlogSubscriberEntityBuilder> _foreignKey = new("FK_BlogSubscribers_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public BlogSubscriberEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_foreignKey);
        }

        protected override BlogSubscriberEntityBuilder BuildTable(ColumnsBuilder table)
        {
            BlogSubscriberId = AddAutoIncrementColumn(table, "BlogSubscriberId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            Email = AddStringColumn(table, "Email", 256, false, true);
            Guid = AddStringColumn(table, "Guid", 36, false, true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> BlogSubscriberId { get; set; }

        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }

        public OperationBuilder<AddColumnOperation> Email { get; set; }

        public OperationBuilder<AddColumnOperation> Guid { get; set; }
    }
}
