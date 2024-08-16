using System;

namespace Oqtane.Blogs.Models
{
    public class BlogSearch
    {
        public string Keywords { get; set; }

        public bool IncludeDraft { get; set; }

        public string Categories { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string SortBy {  get; set; }

        public bool SortByDescending {  get; set; }

        public string Tags { get; set; }

        public string ExcludeBlogs { get; set; }
    }
}
