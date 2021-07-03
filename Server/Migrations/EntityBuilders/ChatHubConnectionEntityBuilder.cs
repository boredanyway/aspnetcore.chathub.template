using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubConnectionEntityBuilder : AuditableBaseEntityBuilder<ChatHubConnectionEntityBuilder>
    {

        private const string _entityTableName = "ChatHubConnection";
        private readonly PrimaryKey<ChatHubConnectionEntityBuilder> _primaryKey = new("PK_ChatHubConnection", x => x.Id);
        private readonly ForeignKey<ChatHubConnectionEntityBuilder> _userForeignKey = new("FK_ChatHubConnection_User", x => x.ChatHubUserId, "User", "UserId", ReferentialAction.Cascade);

        public ChatHubConnectionEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_userForeignKey);
        }

        protected override ChatHubConnectionEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubUserId = AddIntegerColumn(table, "ChatHubUserId");
            ConnectionId = AddStringColumn(table, "ConnectionId", 256, false, true);
            IpAddress = AddStringColumn(table, "IpAddress", 256, false, true);
            UserAgent = AddStringColumn(table, "UserAgent", 512, false, true);
            Status = AddStringColumn(table, "Status", 256, false, true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubUserId { get; set; }

        public OperationBuilder<AddColumnOperation> ConnectionId { get; set; }

        public OperationBuilder<AddColumnOperation> IpAddress { get; set; }

        public OperationBuilder<AddColumnOperation> UserAgent { get; set; }

        public OperationBuilder<AddColumnOperation> Status { get; set; }

    }
}