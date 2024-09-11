using Oqtane.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Blogs.Models
{
    public class BlogTagSource : IAuditable
    {
        public int BlogTagSourceId { get; set; }

        public int ModuleId { get; set; }

        public string Tag { get; set; }

        [NotMapped]
        public int BlogCount { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
