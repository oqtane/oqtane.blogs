using Oqtane.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Blogs.Models
{
    public class BlogTag : IAuditable
    {
        public int BlogTagId { get; set; }

        public int BlogId { get; set; }

        public int BlogTagSourceId { get; set; }

        public BlogTagSource BlogTagSource { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
