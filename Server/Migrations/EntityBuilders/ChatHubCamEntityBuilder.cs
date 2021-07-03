using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubCamEntityBuilder : AuditableBaseEntityBuilder<ChatHubCamEntityBuilder>
    {

        private const string _entityTableName = "ChatHubCam";
        private readonly PrimaryKey<ChatHubCamEntityBuilder> _primaryKey = new("PK_ChatHubRoom", x => x.Id);
        private readonly ForeignKey<ChatHubCamEntityBuilder> _roomForeignKey = new("FK_ChatHubCam_ChatHubRoom", x => x.ChatHubRoomId, "ChatHubRoom", "Id", ReferentialAction.Cascade);
        private readonly ForeignKey<ChatHubCamEntityBuilder> _connectionForeignKey = new("FK_ChatHubCam_ChatHubConnection", x => x.ChatHubConnectionId, "ChatHubConnection", "Id", ReferentialAction.Cascade);

        public ChatHubCamEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_roomForeignKey);
            ForeignKeys.Add(_connectionForeignKey);
        }

        protected override ChatHubCamEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubRoomId = AddIntegerColumn(table, "ChatHubRoomId");
            ChatHubConnectionId = AddIntegerColumn(table, "ChatHubConnectionId");
            Status = AddStringColumn(table, "Status", 256, false, true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubRoomId { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubConnectionId { get; set; }

        public OperationBuilder<AddColumnOperation> Status { get; set; }

    }
}