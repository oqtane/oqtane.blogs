using Oqtane.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oqtane.Blogs.Models
{
    [Table("BlogCategorySource")]
    public class CategorySource : IAuditable
    {
        public int CategorySourceId { get; set; }

        public int ModuleId { get; set; }

        public string Name { get; set; }

        [NotMapped]
        public int BlogCount { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
