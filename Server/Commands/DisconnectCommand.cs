using Microsoft.AspNetCore.SignalR;
using System.Composition;
using System.Threading.Tasks;
using Oqtane.Shared;
using Oqtane.ChatHubs.Models;

namespace Oqtane.ChatHubs.Commands
{
    [Export("ICommand", typeof(ICommand))]
    [Command("disconnect", "[]", new string[] { RoleNames.Everyone, RoleNames.Registered, RoleNames.Admin } , "Usage: /disconnect | /exit | /shutdown")]
    public class DisconnectCommand : BaseCommand
    {
        public override async Task Execute(CommandServicesContext context, CommandCallerContext callerContext, string[] args, ChatHubUser caller)
        {

            await context.ChatHub.Clients.Client(callerContext.ConnectionId).SendAsync("Disconnect", context.ChatHubService.CreateChatHubUserClientModel(caller));

        }
    }
}