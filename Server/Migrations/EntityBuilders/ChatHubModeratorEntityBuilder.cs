using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubModeratorEntityBuilder : AuditableBaseEntityBuilder<ChatHubModeratorEntityBuilder>
    {

        private const string _entityTableName = "ChatHubModerator";
        private readonly PrimaryKey<ChatHubModeratorEntityBuilder> _primaryKey = new("PK_ChatHubRoom", x => x.Id);
        private readonly ForeignKey<ChatHubModeratorEntityBuilder> _userForeignKey = new("FK_ChatHubModerator_User", x => x.ChatHubUserId, "User", "UserId", ReferentialAction.Cascade);

        public ChatHubModeratorEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_userForeignKey);
        }

        protected override ChatHubModeratorEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubUserId = AddIntegerColumn(table, "ChatHubUserId");
            ModeratorDisplayName = AddStringColumn(table, "ModeratorDisplayName", 512, false, true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubUserId { get; set; }

        public OperationBuilder<AddColumnOperation> ModeratorDisplayName { get; set; }

    }
}