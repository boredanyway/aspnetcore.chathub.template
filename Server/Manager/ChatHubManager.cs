using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Shared;
using Oqtane.Migrations.Framework;
using Oqtane.Enums;
using Microsoft.AspNetCore.Http;
using Oqtane.ChatHubs.Repository;
using Oqtane.ChatHubs.Models;

namespace Oqtane.ChatHubs.Manager
{
    public class ChatHubManager : MigratableModuleBase, IInstallable, IPortable
    {

        private ChatHubRepository _chatHubRepository;
        private ISqlRepository _sql;
        private readonly ITenantManager _tenantManager;
        private readonly IHttpContextAccessor _accessor;

        public ChatHubManager(ChatHubRepository chatHubRepository, ISqlRepository sql, ITenantManager tenantManager, IHttpContextAccessor accessor)
        {
            _chatHubRepository = chatHubRepository;
            _sql = sql;
            _tenantManager = tenantManager;
            _accessor = accessor;
        }

        public bool Install(Tenant tenant, string version)
        {
            if (tenant.DBType == Constants.DefaultDBType)
            {
                if (version == "4.0.2009-delta1")
                {
                    // version 1.0.0 used SQL scripts rather than migrations, so we need to seed the migration history table
                    _sql.ExecuteNonQuery(tenant, MigrationUtils.BuildInsertScript("ChatHub.01.00.00.00"));
                }
            }
            return Migrate(new ChatHubContext(_tenantManager, _accessor), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new ChatHubContext(_tenantManager, _accessor), tenant, MigrationType.Down);
        }

        public string ExportModule(Module module)
        {
            string content = "";
            List<ChatHubRoom> rooms = _chatHubRepository.GetChatHubRooms().FilterByModuleId(module.ModuleId).ToList();
            if (rooms != null)
            {
                content = JsonSerializer.Serialize(rooms);
            }
            return content;
        }

        public void ImportModule(Module module, string content, string version)
        {
            List<ChatHubRoom> rooms = null;
            if (!string.IsNullOrEmpty(content))
            {
                rooms = JsonSerializer.Deserialize<List<ChatHubRoom>>(content);
            }
            if (rooms != null)
            {
                foreach (ChatHubRoom room in rooms)
                {
                    ChatHubRoom Room = new ChatHubRoom();
                    Room.ModuleId = module.ModuleId;
                    Room.Id = room.Id;
                    Room.Title = room.Title;
                    Room.Content = room.Content;
                    Room.Status = room.Status;
                    Room.Type = room.Type;
                    Room.CreatorId = room.CreatorId;
                    Room.ImageUrl = room.ImageUrl;
                    Room.OneVsOneId = room.OneVsOneId;
                    Room.BackgroundColor = room.BackgroundColor;

                    _chatHubRepository.AddChatHubRoom(Room);
                }
            }
        }
    }
}