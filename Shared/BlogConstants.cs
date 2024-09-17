using System.Collections.Generic;

namespace Oqtane.Blogs.Shared
{
    public sealed class BlogConstants
    {
        public const int DefaultVersion = 1;

        public const string DefaultColumnCssClass = "col-lg-4 col-md-6";
        public const string DefaultRowCssClass = "row";
        public const string DefaultSummaryTemplate = "<a href=\"[URL]\"><h2>[TITLE]</h2></a><p>[SUMMARY]</p>";
        public const string DefaultDetailTemplate = "<h2>[TITLE]</h2><p>[CONTENT]</p>";
        public const string DefaultCommentTemplate = "<figure><blockquote class=\"blockquote\"><p>[COMMENT]</p></blockquote><figcaption class=\"blockquote-footer\">[NAME] on [DATE] at [TIME]</figcaption></figure>";
        public const string DefaultSearchProperties = "Title,Summary";
        public const string DefaultPagerPosition = "Top";

        public static readonly IList<string> AvailableWidgets = new List<string> { "PopularPosts", "RelatedPosts", "Categories", "Archives", "LatestPosts", "Tags"};
    }
}
