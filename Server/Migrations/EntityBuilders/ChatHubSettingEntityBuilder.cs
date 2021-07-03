using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubSettingEntityBuilder : AuditableBaseEntityBuilder<ChatHubSettingEntityBuilder>
    {

        private const string _entityTableName = "ChatHubSetting";
        private readonly PrimaryKey<ChatHubSettingEntityBuilder> _primaryKey = new("PK_ChatHubSetting", x => x.Id);
        private readonly ForeignKey<ChatHubSettingEntityBuilder> _userForeignKey = new("FK_ChatHubSetting_User", x => x.ChatHubUserId, "User", "UserId", ReferentialAction.Cascade);

        public ChatHubSettingEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_userForeignKey);
        }

        protected override ChatHubSettingEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubUserId = AddIntegerColumn(table, "ChatHubUserId");
            UsernameColor = AddStringColumn(table, "UsernameColor", 256, false, true);
            MessageColor = AddMaxStringColumn(table, "MessageColor");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubUserId { get; set; }

        public OperationBuilder<AddColumnOperation> UsernameColor { get; set; }

        public OperationBuilder<AddColumnOperation> MessageColor { get; set; }

    }
}