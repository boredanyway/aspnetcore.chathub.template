using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubIgnoreEntityBuilder : AuditableBaseEntityBuilder<ChatHubIgnoreEntityBuilder>
    {

        private const string _entityTableName = "ChatHubIgnore";
        private readonly PrimaryKey<ChatHubIgnoreEntityBuilder> _primaryKey = new("PK_ChatHubIgnore", x => x.Id);
        private readonly ForeignKey<ChatHubIgnoreEntityBuilder> _userForeignKey = new("FK_ChatHubIgnore_User", x => x.ChatHubUserId, "User", "UserId", ReferentialAction.Cascade);

        public ChatHubIgnoreEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_userForeignKey);
        }

        protected override ChatHubIgnoreEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubUserId = AddIntegerColumn(table, "ChatHubUserId");
            ChatHubIgnoredUserId = AddIntegerColumn(table, "ChatHubIgnoredUserId");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubUserId { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubIgnoredUserId { get; set; }

    }
}