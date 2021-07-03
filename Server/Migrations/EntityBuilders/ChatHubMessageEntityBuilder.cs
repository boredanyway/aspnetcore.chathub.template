using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubMessageEntityBuilder : AuditableBaseEntityBuilder<ChatHubMessageEntityBuilder>
    {

        private const string _entityTableName = "ChatHubMessage";
        private readonly PrimaryKey<ChatHubMessageEntityBuilder> _primaryKey = new("PK_ChatHubMessage", x => x.Id);
        private readonly ForeignKey<ChatHubMessageEntityBuilder> _roomForeignKey = new("FK_ChatHubRoom", x => x.ChatHubRoomId, "ChatHubRoom", "Id", ReferentialAction.Cascade);
        private readonly ForeignKey<ChatHubMessageEntityBuilder> _userForeignKey = new("FK_ChatHubRoom", x => x.ChatHubUserId, "User", "UserId", ReferentialAction.Cascade);

        public ChatHubMessageEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_roomForeignKey);
            ForeignKeys.Add(_userForeignKey);
        }

        protected override ChatHubMessageEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubRoomId = AddIntegerColumn(table, "ChatHubRoomId");
            ChatHubUserId = AddIntegerColumn(table, "ChatHubUserId");
            Title = AddStringColumn(table, "Title", 256, false, true);
            Content = AddMaxStringColumn(table, "Content");
            Type = AddStringColumn(table, "Type", 256, false, true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubRoomId { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubUserId { get; set; }

        public OperationBuilder<AddColumnOperation> Title { get; set; }

        public OperationBuilder<AddColumnOperation> Content { get; set; }

        public OperationBuilder<AddColumnOperation> Type { get; set; }

    }
}