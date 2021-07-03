using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubBlacklistUserEntityBuilder : AuditableBaseEntityBuilder<ChatHubBlacklistUserEntityBuilder>
    {

        private const string _entityTableName = "ChatHubBlacklistUser";
        private readonly PrimaryKey<ChatHubBlacklistUserEntityBuilder> _primaryKey = new("PK_ChatHubBlacklistUser", x => x.Id);
        private readonly ForeignKey<ChatHubBlacklistUserEntityBuilder> _blacklistUserForeignKey = new("FK_ChatHubBlacklistUser_User", x => x.ChatHubUserId, "User", "UserId", ReferentialAction.Cascade);

        public ChatHubBlacklistUserEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_blacklistUserForeignKey);
        }

        protected override ChatHubBlacklistUserEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubUserId = AddIntegerColumn(table, "ChatHubUserId");
            BlacklistUserDisplayName = AddStringColumn(table, "BlacklistUserDisplayName", 512, false, true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubUserId { get; set; }

        public OperationBuilder<AddColumnOperation> BlacklistUserDisplayName { get; set; }

    }
}