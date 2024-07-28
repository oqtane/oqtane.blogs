﻿using Oqtane.Blogs.Models;
using Oqtane.Models;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oqtane.Blogs.Shared
{
    public sealed class BlogUtilities
    {
        private const string ItemTemlateStartTag = "<#itemtemplate>";
        private const string ItemTemlateEndTag = "</#itemtemplate>";

        public static string FormatUrl(string alias, string path, Blog blog)
        {
            return Utilities.NavigateUrl(alias, path, Utilities.AddUrlParameters(blog.Slug));
        }

        public static string FormatDate(DateTime date, string format)
        {
            if (date == DateTime.MinValue)
            {
                return string.Empty;
            }

            return date.ToString(format);
        }

        public static string FormatThumbnail(Blog blog)
        {
            if (string.IsNullOrEmpty(blog.Thumbnail))
            {
                return string.Empty;
            }

            return $"<img src=\"{blog.Thumbnail}\" alt=\"{blog.AlternateText}\">";
        }

        public static string FormatCategories(Blog blog, string baseUrl)
        {
            if (!blog.BlogCategories.Any())
            {
                return string.Empty;
            }

            var categories = new StringBuilder("<ul class=\"blog-categories-list\">");
            foreach (var category in blog.BlogCategories)
            {
                categories.Append($"<li><a href=\"{baseUrl}?Category={category.CategorySourceId}\">{category.CategorySource.Name}</a>");
            }
            categories.Append("</ul>");

            return categories.ToString();
        }

        public static string FormatTags(Blog blog, string baseUrl)
        {
            if (!blog.BlogTags.Any())
            {
                return string.Empty;
            }

            var tags = new StringBuilder("<ul class=\"blog-tags-list\">");
            foreach (var tag in blog.BlogTags.OrderBy(i => i.TagSource.Tag))
            {
                tags.Append($"<li><a href=\"{baseUrl}?Tag={tag.TagSource.Tag}\">{tag.TagSource.Tag}</a>");
            }
            tags.Append("</ul>");

            return tags.ToString();
        }

        public static bool ParseListTemplate(string template, out string itemTemplate, out string headTemplate, out string footTemplate)
        {
            var startIndex = template.IndexOf(ItemTemlateStartTag, StringComparison.OrdinalIgnoreCase);
            var endIndex = template.IndexOf(ItemTemlateEndTag, StringComparison.OrdinalIgnoreCase);
            itemTemplate = string.Empty;
            headTemplate = string.Empty;
            footTemplate = string.Empty;
            if (startIndex > -1 && endIndex > -1)
            {
                headTemplate = template.Substring(0, startIndex);
                footTemplate = template.Substring(endIndex + ItemTemlateEndTag.Length);
                itemTemplate = template.Substring(startIndex + ItemTemlateStartTag.Length, endIndex - startIndex - ItemTemlateStartTag.Length);

                return true;
            }

            return false;
        }
    }
}
