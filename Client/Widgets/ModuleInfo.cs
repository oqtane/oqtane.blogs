using Oqtane.Models;
using Oqtane.Modules;

namespace Oqtane.Blogs.Widgets
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Blog Widgets",
            Description = "Blog Widgets",
            Version = "5.1.0",
            ReleaseVersions = "5.1.0",
            ServerManagerType = string.Empty,
            Dependencies = "Oqtane.Blogs.Shared.Oqtane",
            SettingsType = "Oqtane.Blogs.Widgets.Settings, Oqtane.Blogs.Client.Oqtane",
            PackageName = "Oqtane.Blogs.Widgets"
        };
    }
}
