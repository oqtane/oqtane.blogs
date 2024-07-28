using Oqtane.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oqtane.Blogs.Models
{
    [Table("BlogTag")]
    public class BlogTag : IAuditable
    {
        public int BlogTagId { get; set; }

        public int BlogId { get; set; }

        public int TagSourceId { get; set; }

        public TagSource TagSource { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
