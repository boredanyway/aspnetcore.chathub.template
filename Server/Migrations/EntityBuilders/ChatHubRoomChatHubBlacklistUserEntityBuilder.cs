using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubRoomChatHubBlacklistUserEntityBuilder : AuditableBaseEntityBuilder<ChatHubRoomChatHubBlacklistUserEntityBuilder>
    {

        private const string _entityTableName = "ChatHubRoomChatHubBlacklistUser";
        private readonly PrimaryKey<ChatHubRoomChatHubBlacklistUserEntityBuilder> _primaryKey = new("PK_ChatHubRoomChatHubBlacklistUser", x => x.Id);
        private readonly ForeignKey<ChatHubRoomChatHubBlacklistUserEntityBuilder> _roomForeignKey = new("FK_ChatHubRoomChatHubBlacklistUser_ChatHubRoom", x => x.ChatHubRoomId, "ChatHubRoom", "Id", ReferentialAction.Cascade);
        private readonly ForeignKey<ChatHubRoomChatHubBlacklistUserEntityBuilder> _blacklistUserForeignKey = new("FK_ChatHubRoomChatHubBlacklistUser_ChatHubBlacklistUser", x => x.ChatHubBlacklistUserId, "ChatHubBlacklistUser", "Id", ReferentialAction.Cascade);

        public ChatHubRoomChatHubBlacklistUserEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_roomForeignKey);
            ForeignKeys.Add(_blacklistUserForeignKey);
        }

        protected override ChatHubRoomChatHubBlacklistUserEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubRoomId = AddIntegerColumn(table, "ChatHubRoomId");
            ChatHubBlacklistUserId = AddIntegerColumn(table, "ChatHubBlacklistUserId");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubRoomId { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubBlacklistUserId { get; set; }

    }
}