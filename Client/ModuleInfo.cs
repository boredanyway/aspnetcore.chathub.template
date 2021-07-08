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
            Version = "2.2.0",
            ServerManagerType = "Oqtane.ChatHubs.Manager.ChatHubManager, Oqtane.ChatHubs.Server.Oqtane",
            ReleaseVersions = "2.2.0",
            Dependencies = "Oqtane.ChatHubs.Shared.Oqtane"
        };
    }
}
