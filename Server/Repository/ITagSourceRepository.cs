using System.Collections.Generic;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public interface ITagSourceRepository
    {
        IEnumerable<TagSource> GetTagSources(int moduleId);
        TagSource AddTagSource(TagSource TagSource);
        TagSource UpdateTagSource(TagSource TagSource);
        void DeleteTagSource(int TagSourceId);
    }
}
