using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Oqtane.ChatHubs.Migrations.EntityBuilders
{
    public class ChatHubPhotoEntityBuilder : AuditableBaseEntityBuilder<ChatHubPhotoEntityBuilder>
    {

        private const string _entityTableName = "ChatHubPhoto";
        private readonly PrimaryKey<ChatHubPhotoEntityBuilder> _primaryKey = new("PK_ChatHubPhoto", x => x.Id);
        private readonly ForeignKey<ChatHubPhotoEntityBuilder> _photoForeignKey = new("FK_ChatHubPhoto_ChatHubMessage", x => x.ChatHubMessageId, "ChatHubMessage", "Id", ReferentialAction.Cascade);

        public ChatHubPhotoEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_photoForeignKey);
        }

        protected override ChatHubPhotoEntityBuilder BuildTable(ColumnsBuilder table)
        {
            Id = AddAutoIncrementColumn(table, "Id");
            ChatHubMessageId = AddIntegerColumn(table, "ChatHubMessageId");
            Source = AddStringColumn(table, "Source", 256, false, true);
            Thumb = AddStringColumn(table, "Thumb", 256, false, true);
            Caption = AddStringColumn(table, "Caption", 512, false, true);
            Size = AddIntegerColumn(table, "Size");
            Width = AddIntegerColumn(table, "Width");
            Height = AddIntegerColumn(table, "Height");

            AddAuditableColumns(table);

            return this;
        }

        public OperationBuilder<AddColumnOperation> Id { get; set; }

        public OperationBuilder<AddColumnOperation> ChatHubMessageId { get; set; }

        public OperationBuilder<AddColumnOperation> Source { get; set; }

        public OperationBuilder<AddColumnOperation> Thumb { get; set; }

        public OperationBuilder<AddColumnOperation> Caption { get; set; }

        public OperationBuilder<AddColumnOperation> Size { get; set; }

        public OperationBuilder<AddColumnOperation> Width { get; set; }

        public OperationBuilder<AddColumnOperation> Height { get; set; }

    }
}