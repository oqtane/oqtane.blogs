using System;
using Oqtane.Models;

namespace Oqtane.Blogs.Models
{
    public class BlogSubscriber : IAuditable
    {
        public int BlogSubscriberId { get; set; }
        public int ModuleId { get; set; }
        public string Email { get; set; }
        public string Guid { get; set; }
        public bool IsVerified { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
