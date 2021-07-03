using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubRoomChatHubUserEntityBuilder : AuditableBaseEntityBuilder<ChatHubRoomChatHubUserEntityBuilder>
    {

        private const string _entityTableName = "ChatHubRoomChatHubUser";
        private readonly PrimaryKey<ChatHubRoomChatHubUserEntityBuilder> _primaryKey = new("PK_ChatHubRoomChatHubUser", x => x.Id);
        private readonly ForeignKey<ChatHubRoomChatHubUserEntityBuilder> _userForeignKey = new("FK_ChatHubRoomChatHubUser_ChatHubUser", x => x.ChatHubUserId, "User", "UserId", ReferentialAction.Cascade);
        private readonly ForeignKey<ChatHubRoomChatHubUserEntityBuilder> _roomForeignKey = new("FK_ChatHubRoomChatHubUser_ChatHubRoom", x => x.ChatHubRoomId, "ChatHubRoom", "Id", ReferentialAction.Cascade);

        public ChatHubRoomChatHubUserEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_userForeignKey);
            ForeignKeys.Add(_roomForeignKey);
        }

        protected override ChatHubRoomChatHubUserEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubRoomId = AddIntegerColumn(table, "ChatHubRoomId");
            ChatHubUserId = AddIntegerColumn(table, "ChatHubUserId");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubRoomId { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubUserId { get; set; }

    }
}