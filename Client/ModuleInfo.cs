using Oqtane.Models;
using Oqtane.Modules;

namespace Oqtane.ChatHubs
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "ChatHub",
            Description = "ChatHub",
            Version = "2.1.0",
            ServerManagerType = "Oqtane.ChatHubs.Manager.ChatHubManager, Oqtane.ChatHubs.Server.Oqtane",
            ReleaseVersions = "2.1.0",
            Dependencies = "Oqtane.ChatHubs.Shared.Oqtane"
        };
    }
}
