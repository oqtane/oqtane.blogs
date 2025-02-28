using Oqtane.Models;
using Oqtane.Modules;

namespace Oqtane.Blogs.Widgets
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Blog Widgets",
            Description = "Widgets For Displaying Blog Posts In A Variety Of Formats",
            Version = "6.0.2",
            Dependencies = "Oqtane.Blogs.Shared.Oqtane",
            SettingsType = "Oqtane.Blogs.Widgets.Settings, Oqtane.Blogs.Client.Oqtane",
            PackageName = "Oqtane.Blogs.Widgets"
        };
    }
}
