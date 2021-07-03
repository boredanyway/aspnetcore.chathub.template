using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubWhitelistUserEntityBuilder : AuditableBaseEntityBuilder<ChatHubWhitelistUserEntityBuilder>
    {

        private const string _entityTableName = "ChatHubWhitelistUser";
        private readonly PrimaryKey<ChatHubWhitelistUserEntityBuilder> _primaryKey = new("PK_ChatHubWhitelistUser", x => x.Id);
        private readonly ForeignKey<ChatHubWhitelistUserEntityBuilder> _whitelistUserForeignKey = new("FK_ChatHubWhitelistUser_User", x => x.ChatHubUserId, "User", "UserId", ReferentialAction.Cascade);

        public ChatHubWhitelistUserEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_whitelistUserForeignKey);
        }

        protected override ChatHubWhitelistUserEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubUserId = AddIntegerColumn(table, "ChatHubUserId");
            WhitelistUserDisplayName = AddStringColumn(table, "WhitelistUserDisplayName", 512, false, true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubUserId { get; set; }

        public OperationBuilder<AddColumnOperation> WhitelistUserDisplayName { get; set; }

    }
}