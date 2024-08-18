using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using Oqtane.Blogs.Models;
using Oqtane.Blogs.Shared;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Oqtane.Blogs.Widgets
{
    public abstract class WidgetBase : ModuleBase
    {
        [Inject]
        public IPageService PageService { get; set; }

        [Inject]
        public IModuleService ModuleService { get; set; }

        [Parameter]
        public int BlogPageId { get; set; }

        [Parameter]
        public int BlogModuleId { get; set; }

        [Parameter]
        public string Template { get; set; }

        [Parameter]
        public int ItemsCount { get; set; }

        protected string HeadTemplate { get; set; }

        protected string ItemTemplate { get; set; }

        protected string FootTemplate { get; set; }

        protected IList<Blog> Blogs { get; set; }

        protected async Task<Page> GetBlogPage()
        {
            return await PageService.GetPageAsync(BlogPageId);
        }

        protected async Task<string> GetBlogUrl()
        {
            var page = await GetBlogPage();
            if (page != null)
            {
                return NavigateUrl(page.Path);
            }

            return string.Empty;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(Template))
            {
                if (BlogUtilities.ParseListTemplate(Template, out string itemTemplate, out string headTemplate, out string footTemplate))
                {
                    ItemTemplate = itemTemplate;
                    HeadTemplate = headTemplate;
                    FootTemplate = footTemplate;
                }
            }

            await Task.CompletedTask;
        }

        protected virtual async Task<IDictionary<string, string>> GenerateCommonTokens()
        {
            var tokens = new Dictionary<string, string>();
            tokens.Add("BLOGURL", await GetBlogUrl());

            return tokens;
        }

        protected virtual async Task<IList<IDictionary<string, string>>> GenerateItemTokens()
        {
            var tokensList = new List<IDictionary<string, string>>();
            
            if(Blogs != null && Blogs.Any())
            {
                var blogPage = await GetBlogPage();
                var blogPageUrl = Utilities.NavigateUrl(PageState.Alias.Path, blogPage.Path, string.Empty);

                foreach (var blog in Blogs)
                {
                    var blogContent = PageState.EditMode ? blog.LatestBlogContent : blog.PublishedBlogContent;
                    if (blogContent != null)
                    {
                        var publishDate = blogContent.PublishDate.GetValueOrDefault(blogContent.CreatedOn);
                        var thumbnailPath = blog.Thumbnail > 0 ? Utilities.FileUrl(PageState.Alias, blog.Thumbnail) : string.Empty;

                        var tokens = new Dictionary<string, string>
                        {
                            { "ID", blog.BlogId.ToString() },
                            { "TITLE", blog.Title },
                            { "SLUG", blog.Slug },
                            { "VIEWS", blog.Views.ToString() },
                            { "THUMBNAILPATH", thumbnailPath },
                            { "ALTERNATETEXT", blog.AlternateText },
                            { "THUMBNAIL", BlogUtilities.FormatThumbnail(blog, thumbnailPath) },
                            { "CATEGORIES", BlogUtilities.FormatCategories(blog, blogPageUrl) },
                            { "TAGS", BlogUtilities.FormatTags(blog, blogPageUrl) },
                            { "SUMMARY", blogContent.Summary },
                            { "CONTENT", Utilities.FormatContent(blogContent.Content, PageState.Alias, "render") },
                            { "CREATEDBY", blog.CreatedBy },
                            { "CREATEDON", blog.CreatedOn.ToShortDateString() },
                            { "URL", blogPage != null ? BlogUtilities.FormatUrl(PageState.Alias.Path, blogPage.Path, blog) : string.Empty },
                            { "PUBLISHEDON", BlogUtilities.FormatDate(publishDate, "MMMM dd, yyyy") },
                            { "PUBLISHDAY", BlogUtilities.FormatDate(publishDate, "dd") },
                            { "PUBLISHMONTH", BlogUtilities.FormatDate(publishDate, "MMMM") }
                        };

                        tokensList.Add(tokens);
                    }
                }
            }

            return tokensList;
        }

        protected virtual async Task<string> RenderItems()
        {
            await Task.CompletedTask;

            return string.Empty; 
        }

        protected virtual async Task<string> RenderTemplate()
        {
            var commonTokens = await GenerateCommonTokens();

            if (string.IsNullOrEmpty(ItemTemplate)) //no specific item template, render the generic template
            {
                commonTokens.Add("ITEMS", await RenderItems());

                return ReplaceTemplateTokens(Template, commonTokens);
            }
            else
            {
                var contentBuilder = new StringBuilder();
                contentBuilder.Append(ReplaceTemplateTokens(HeadTemplate, commonTokens));
                var itemTokens = await GenerateItemTokens();
                foreach (var itemToken in itemTokens)
                {
                    var tokens = MergeTokens(commonTokens, itemToken);
                    contentBuilder.Append(ReplaceTemplateTokens(ItemTemplate, tokens));
                }
                contentBuilder.Append(ReplaceTemplateTokens(FootTemplate, commonTokens));

                return contentBuilder.ToString();
            }
        }

        private string ReplaceTemplateTokens(string template, IDictionary<string, string> tokens)
        {
            if(string.IsNullOrEmpty(template))
            {
                return string.Empty;
            }

            var renderContent = template;
            foreach (var key in tokens.Keys)
            {
                renderContent = renderContent.Replace($"[{key.ToUpper()}]", tokens[key], StringComparison.OrdinalIgnoreCase);
            }

            return renderContent;
        }

        private IDictionary<string, string> MergeTokens(IDictionary<string, string> baseTokens, IDictionary<string, string> overrideTokens)
        {
            var tokens = new Dictionary<string, string>();
            foreach (var key in baseTokens.Keys)
            {
                tokens.Add(key, baseTokens[key]);
            }

            foreach (var key in overrideTokens.Keys)
            {
                if (tokens.ContainsKey(key))
                {
                    tokens[key] = overrideTokens[key];
                }
                else
                {
                    tokens.Add(key, overrideTokens[key]);
                }
            }

            return tokens;
        }
    }
}
