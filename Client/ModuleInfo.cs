using Oqtane.Models;
using Oqtane.Modules;

namespace Oqtane.Blogs
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Blog",
            Description = "Blog",
            Version = "1.0.5",
            ServerManagerType = "Oqtane.Blogs.Manager.BlogManager, Oqtane.Blogs.Server.Oqtane",
            ReleaseVersions = "1.0.0,1.0.1,1.0.3,1.0.4,1.0.5",
            Dependencies = "Oqtane.Blogs.Shared.Oqtane",
            SettingsType = "Oqtane.Blogs.Settings, Oqtane.Blogs.Client.Oqtane"
        };
    }
}
