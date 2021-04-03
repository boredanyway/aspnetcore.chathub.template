using Microsoft.AspNetCore.Identity;
using Oqtane.ChatHubs.Hubs;
using Oqtane.ChatHubs.Repository;
using Oqtane.ChatHubs.Services;

namespace Oqtane.ChatHubs.Commands
{
    public class CommandServicesContext
    {
        public ChatHub ChatHub { get; set; }
        public ChatHubRepository ChatHubRepository { get; set; }
        public ChatHubService ChatHubService { get; set; }
        public UserManager<IdentityUser> UserManager { get; set; }
    }
}