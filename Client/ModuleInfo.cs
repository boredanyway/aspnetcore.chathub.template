using Oqtane.Models;
using Oqtane.Modules;

namespace Oqtane.ChatHubs
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "ChatHub",
            Description = "Asp net core signalr oqtane chathub module",
            Version = "1.1.1",
            ServerManagerType = "Oqtane.ChatHubs.Manager.ChatHubManager, Oqtane.ChatHubs.Server.Oqtane",
            ReleaseVersions = "1.1.1",
            Dependencies = "Oqtane.ChatHubs.Shared.Oqtane"
        };
    }
}
