using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.ChatHubs.Migrations.EntityBuilders;
using Oqtane.ChatHubs.Repository;

namespace Oqtane.ChatHubs.Migrations
{
    [DbContext(typeof(ChatHubContext))]
    [Migration("ChatHub.01.00.00.00")]
    public class InitializeModule : MultiDatabaseMigration
    {
        public InitializeModule(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {

            var entityBuilderRoom = new ChatHubRoomEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilderRoom.Create();

            var entityBuilderRoomUser = new ChatHubRoomChatHubUserEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilderRoomUser.Create();

            var entityBuilderMessage = new ChatHubMessageEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilderMessage.Create();

            var entityBuilderConnection = new ChatHubConnectionEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilderConnection.Create();

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new ChatHubRoomEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Drop();
        }
    }
}
