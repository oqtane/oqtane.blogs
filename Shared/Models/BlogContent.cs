using System;
using Oqtane.Blogs.Shared;
using Oqtane.Models;

namespace Oqtane.Blogs.Models
{
    public class BlogContent : IAuditable
    {
        public int BlogContentId { get; set; }

        public int BlogId { get; set; }

        public int Version { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public bool IsPublished { get; set; }

        public DateTime? PublishDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
