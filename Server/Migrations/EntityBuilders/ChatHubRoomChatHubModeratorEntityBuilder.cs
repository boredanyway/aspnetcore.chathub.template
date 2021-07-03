using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubRoomChatHubModeratorEntityBuilder : AuditableBaseEntityBuilder<ChatHubRoomChatHubModeratorEntityBuilder>
    {

        private const string _entityTableName = "ChatHubRoomChatHubModerator";
        private readonly PrimaryKey<ChatHubRoomChatHubModeratorEntityBuilder> _primaryKey = new("PK_ChatHubRoomChatHubModerator", x => x.Id);
        private readonly ForeignKey<ChatHubRoomChatHubModeratorEntityBuilder> _roomForeignKey = new("FK_ChatHubRoomChatHubModerator_ChatHubRoom", x => x.ChatHubRoomId, "ChatHubRoom", "Id", ReferentialAction.Cascade);
        private readonly ForeignKey<ChatHubRoomChatHubModeratorEntityBuilder> _moderatorForeignKey = new("FK_ChatHubRoomChatHubModerator_ChatHubModerator", x => x.ChatHubModeratorId, "ChatHubModerator", "Id", ReferentialAction.Cascade);

        public ChatHubRoomChatHubModeratorEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_roomForeignKey);
            ForeignKeys.Add(_moderatorForeignKey);
        }

        protected override ChatHubRoomChatHubModeratorEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubRoomId = AddIntegerColumn(table, "ChatHubRoomId");
            ChatHubModeratorId = AddIntegerColumn(table, "ChatHubModeratorId");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubRoomId { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubModeratorId { get; set; }

    }
}