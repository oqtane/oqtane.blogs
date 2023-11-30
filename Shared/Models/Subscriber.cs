using System;
using Oqtane.Models;

namespace Oqtane.Blogs.Models
{
    public class Subscriber : IAuditable
    {
        public int SubscriberId { get; set; }
        public int ModuleId { get; set; }
        public string Email { get; set; }
        public string Guid { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
