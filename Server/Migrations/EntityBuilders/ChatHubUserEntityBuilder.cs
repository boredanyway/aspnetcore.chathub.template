using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubUserEntityBuilder : AuditableBaseEntityBuilder<ChatHubUserEntityBuilder>
    {

        private const string _entityTableName = "User";        

        public ChatHubUserEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
        }

        protected override ChatHubUserEntityBuilder BuildTable(ColumnsBuilder table)
        {
            UserType = AddStringColumn(table, "UserType", 256, true, true);
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> UserType { get; set; }

    }
}