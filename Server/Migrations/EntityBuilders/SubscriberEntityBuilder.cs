using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.Blogs.Migrations.EntityBuilders
{
    public class SubscriberEntityBuilder : AuditableBaseEntityBuilder<SubscriberEntityBuilder>
    {
        private const string _entityTableName = "Subscriber";
        private readonly PrimaryKey<SubscriberEntityBuilder> _primaryKey = new("PK_Subscriber", x => x.SubscriberId);
        private readonly ForeignKey<SubscriberEntityBuilder> _moduleForeignKey = new("FK_BlogSubscriber_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public SubscriberEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override SubscriberEntityBuilder BuildTable(ColumnsBuilder table)
        {
            SubscriberId = AddAutoIncrementColumn(table, "SubscriberId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            Email = AddStringColumn(table, "Email", 256, false, true);
            Guid = AddStringColumn(table, "Guid", 36, false, true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> SubscriberId { get; set; }

        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }

        public OperationBuilder<AddColumnOperation> Email { get; set; }

        public OperationBuilder<AddColumnOperation> Guid { get; set; }
    }
}
