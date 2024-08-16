using Oqtane.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oqtane.Blogs.Models
{
    [Table("BlogCategory")]
    public class BlogCategory : IAuditable
    {
        public int BlogCategoryId { get; set; }

        public int BlogId { get; set; }

        public int BlogCategorySourceId { get; set; }

        public BlogCategorySource BlogCategorySource { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
