using Oqtane.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oqtane.Blogs.Models
{
    public class BlogComment : ModelBase
    {
        public int BlogCommentId { get; set; }

        public int BlogId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Comment { get; set; }

        public bool IsPublished { get; set; }

        [NotMapped]
        public string PagePath {  get; set; }   
    }
}
