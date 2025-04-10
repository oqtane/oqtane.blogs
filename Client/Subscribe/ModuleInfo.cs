using Oqtane.Models;
using Oqtane.Modules;

namespace Oqtane.Blogs.Subscribe
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Blog Subscription",
            Description = "Blog Subscription Form",
            Version = "6.1.2",
            PackageName = "Oqtane.Blogs"
        };
    }
}
