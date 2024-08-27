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
            Version = "5.2.1",
            ReleaseVersions = "1.0.0,1.0.1,1.0.3,1.0.4,1.0.5,1.0.6,5.0.0,5.0.1,5.0.2,5.0.3,5.1.0,5.2.0,5.2.1",
            ServerManagerType = "Oqtane.Blogs.Manager.BlogManager, Oqtane.Blogs.Server.Oqtane",
            Dependencies = "Oqtane.Blogs.Shared.Oqtane",
            SettingsType = "Oqtane.Blogs.Settings, Oqtane.Blogs.Client.Oqtane",
            PackageName = "Oqtane.Blogs"
        };
    }
}
