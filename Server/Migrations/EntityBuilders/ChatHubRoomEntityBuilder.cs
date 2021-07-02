using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubRoomEntityBuilder : AuditableBaseEntityBuilder<ChatHubRoomEntityBuilder>
    {

        private const string _entityTableName = "ChatHubRoom";
        private readonly PrimaryKey<ChatHubRoomEntityBuilder> _primaryKey = new("PK_ChatHubRoom", x => x.Id);
        private readonly ForeignKey<ChatHubRoomEntityBuilder> _moduleForeignKey = new("FK_ChatHubRoom_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public ChatHubRoomEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override ChatHubRoomEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            Title = AddStringColumn(table, "Title", 256, false, true);
            Content = AddMaxStringColumn(table, "Content");
            BackgroundColor = AddStringColumn(table, "BackgroundColor", 256, false, true);
            ImageUrl = AddStringColumn(table, "ImageUrl", 256, false, true);
            Type = AddStringColumn(table, "Type", 256, false, true);
            Status = AddStringColumn(table, "Status", 256, false, true);
            OneVsOneId = AddStringColumn(table, "OneVsOneId", 256, false, true);

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }

        public OperationBuilder<AddColumnOperation> Title { get; set; }

        public OperationBuilder<AddColumnOperation> Content { get; set; }

        public OperationBuilder<AddColumnOperation> BackgroundColor { get; set; }

        public OperationBuilder<AddColumnOperation> ImageUrl { get; set; }

        public OperationBuilder<AddColumnOperation> Type { get; set; }

        public OperationBuilder<AddColumnOperation> Status { get; set; }

        public OperationBuilder<AddColumnOperation> OneVsOneId { get; set; }

    }
}