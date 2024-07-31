using Oqtane.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oqtane.Blogs.Models
{
    [Table("BlogTagSource")]
    public class TagSource : IAuditable
    {
        public int TagSourceId { get; set; }

        public int ModuleId { get; set; }

        public string Tag { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
