using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Blogs.Models;
using Oqtane.Blogs.Repository;
using Microsoft.AspNetCore.Http;
using Oqtane.Controllers;
using Oqtane.Repository;
using Oqtane.Models;
using Oqtane.Blogs.Shared;
using Oqtane.Extensions;
using System.Linq;
using System;
using System.Net;

namespace Oqtane.Blogs.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BlogController : ModuleControllerBase
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IBlogSubscriberRepository _blogSubscriberRepository;
        private readonly INotificationRepository _notificationRepository;

        public BlogController(IBlogRepository blogRepository, ISiteRepository siteRepository, ISettingRepository settingRepository, IBlogSubscriberRepository blogSubscriberRepository, INotificationRepository notificationRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _blogRepository = blogRepository;
            _siteRepository = siteRepository;
            _settingRepository = settingRepository;
            _blogSubscriberRepository = blogSubscriberRepository;
            _notificationRepository = notificationRepository;
        }

        // GET: api/<controller>?moduleid=x&search=y
        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<Blog> Get(string moduleid, [FromQuery] BlogSearch searchQuery)
        {
            if (int.Parse(moduleid) == _authEntityId[EntityNames.Module])
            {
                return _blogRepository.GetBlogs(int.Parse(moduleid), searchQuery);
            }
            else
            {
                return null;
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize(Policy = "ViewModule")]
        public Blog Get(int id)
        {
            Blog Blog = _blogRepository.GetBlog(id);
            if (Blog != null && Blog.ModuleId != _authEntityId[EntityNames.Module])
            {
                Blog = null;
            }
            return Blog;
        }

        // GET api/<controller>/hello-world
        [HttpGet("slug/{slug}")]
        [Authorize(Policy = "ViewModule")]
        public Blog Get(string slug)
        {
            var blog = _blogRepository.GetBlogBySlug(slug);
            if (blog != null && blog.ModuleId != _authEntityId[EntityNames.Module])
            {
                blog = null;
            }
            return blog;
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public Blog Post([FromBody] Blog blog)
        {
            if (ModelState.IsValid && blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                blog = _blogRepository.AddBlog(blog);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Added {Blog}", blog);
            }
            return blog;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "EditModule")]
        public Blog Put(int id, [FromBody] Blog blog)
        {
            if (ModelState.IsValid && blog.BlogId == id && blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                blog = _blogRepository.UpdateBlog(blog);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Blog Updated {Blog}", blog);
            }
            return blog;
        }

        // PUT api/<controller>?5
        [HttpPut]
        [IgnoreAntiforgeryToken]
        [Authorize(Policy = "ViewModule")]
        public void Put(int id)
        {
            var blog = _blogRepository.GetBlog(id);
            if (blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                blog.Views += 1;
                _blogRepository.UpdateBlog(blog);
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "EditModule")]
        public void Delete(int id)
        {
            Blog Blog = _blogRepository.GetBlog(id);
            if (Blog != null && Blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                _blogRepository.DeleteBlog(id);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Blog Deleted {BlogId}", id);
            }
        }

        [HttpGet("notify/{id}")]
        [Authorize(Policy = "EditModule")]
        public int Notify(int id)
        {
            var subscribers = 0;
            var blog = _blogRepository.GetBlog(id);
            if (blog != null && blog.ModuleId == _authEntityId[EntityNames.Module] && blog.PublishedBlogContent != null)
            {
                var settings = _settingRepository.GetSettings(EntityNames.Module, blog.ModuleId);
                if (settings.Any(item => item.SettingName == "Subscriptions") && settings.First(item => item.SettingName == "Subscriptions").SettingValue == "True")
                {
                    var alias = HttpContext.GetAlias();
                    var rooturl = alias.Protocol + alias.Name;
                    var pagepath = settings.First(item => item.SettingName == "PagePath").SettingValue;
                    var sender = settings.First(item => item.SettingName == "Sender").SettingValue;
                    var parameters = Utilities.AddUrlParameters(blog.BlogId, Common.FormatSlug(blog.Title));
                    var url = rooturl + Utilities.NavigateUrl(alias.Path, pagepath, parameters);

                    // this will likely need to be asynchronous in the future as it may timeout
                    foreach (var subscriber in _blogSubscriberRepository.GetBlogSubscribers(blog.ModuleId))
                    {
                        if (subscriber.IsVerified)
                        {
                            var body = blog.PublishedBlogContent.Summary;
                            body += $"<br /><br />Read Full Article: <a href=\"{url}\">{url}</a>";
                            var unsubscribe = rooturl + Utilities.NavigateUrl(alias.Path, pagepath, "guid=" + subscriber.Guid + "&action=unsubscribe");
                            body += $"<br /><br />Unsubscribe: <a href=\"{unsubscribe}\">{unsubscribe}</a>";
                            var notification = new Notification(alias.SiteId, "", sender, "", subscriber.Email, blog.Title, body);
                            _notificationRepository.AddNotification(notification);
                            subscribers++;
                        }
                    }
                }
            }
            return subscribers;
        }

        [HttpGet("rss/{id}")]
        public IActionResult RSS(int id)
        {
            var rss = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" + Environment.NewLine;

            var settings = _settingRepository.GetSettings(EntityNames.Module, id);
            if (settings.Any(item => item.SettingName == "PagePath"))
            {
                var pagepath = "/" + settings.First(item => item.SettingName == "PagePath").SettingValue;
                var alias = HttpContext.GetAlias();
                var rooturl = alias.Protocol + alias.Name;
                var site = _siteRepository.GetSite(alias.SiteId);

                rss += "<rss version=\"2.0\">" + Environment.NewLine;
                rss += "<channel>" + Environment.NewLine;
                rss += "<title>" + WebUtility.HtmlEncode(site.Name) + "</title>" + Environment.NewLine;
                rss += "<link>" + rooturl + pagepath + "</link>" + Environment.NewLine;
                rss += "<description>" + WebUtility.HtmlEncode(site.Name) + "</description>" + Environment.NewLine;

                var blogs = _blogRepository.GetBlogs(id, null).ToList();
                foreach (var blog in blogs.Where(item => item.PublishedBlogContent != null))
                {
                    rss += "<item>" + Environment.NewLine;
                    rss += "<title>" + WebUtility.HtmlEncode(blog.Title) + "</title>" + Environment.NewLine;
                    var parameters = Utilities.AddUrlParameters(blog.BlogId, Common.FormatSlug(blog.Title));
                    rss += "<link>" + rooturl + Utilities.NavigateUrl(alias.Path, pagepath, parameters) + "</link>" + Environment.NewLine;
                    rss += "<description>" + WebUtility.HtmlEncode(blog.PublishedBlogContent.Summary) + "</description>" + Environment.NewLine;
                    rss += "</item>" + Environment.NewLine;
                }

                rss += "</channel>" + Environment.NewLine;
                rss += "</rss>" + Environment.NewLine;
            }

            return Content(rss, "application/xml");
        }
    }
}
