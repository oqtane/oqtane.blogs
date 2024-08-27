using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Oqtane.Models;

namespace Oqtane.Blogs.Models
{
    public class Blog : IAuditable
    {
        private string _tags;
        private string _categories;

        public int BlogId { get; set; }
        public int ModuleId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Thumbnail { get; set; }
        public string AlternateText { get; set; }
        public bool AllowComments { get; set; }
        public int Views { get; set; }
        public List<BlogContent> BlogContentList { get; set; }
        public List<BlogCategory> BlogCategories { get; set; }
        public List<BlogTag> BlogTags { get; set; }

        [NotMapped]
        public BlogContent PublishedBlogContent
        {
            get
            {
                return BlogContentList?
                    .OrderByDescending(i => i.Version)
                    .FirstOrDefault(i => i.IsPublished && (i.PublishDate == null || i.PublishDate <= DateTime.UtcNow));
            }
        }

        [NotMapped]
        public BlogContent LatestBlogContent
        {
            get
            {
                return BlogContentList?
                    .OrderByDescending(i => i.Version)
                    .FirstOrDefault();
            }
        }

        [NotMapped]
        public string Tags
        {
            get
            {
                if (_tags == null && BlogTags != null)
                {
                    _tags = string.Join(",", BlogTags.Select(i => i.BlogTagSource.Tag));
                }

                return _tags;
            }
            set
            { 
                _tags = value;
            }
        }

        [NotMapped]
        public string Categories
        {
            get
            {
                if (_categories == null && BlogCategories != null)
                {
                    _categories = string.Join(",", BlogCategories.Select(i => i.BlogCategorySourceId));
                }

                return _categories;
            }
            set
            {
                _categories = value;
            }
        }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
