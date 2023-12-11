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
        private readonly IBlogRepository _BlogRepository;
        private readonly ISiteRepository _SiteRepository;
        private readonly ISettingRepository _SettingRepository;
        private readonly ISubscriberRepository _SubscriberRepository;
        private readonly INotificationRepository _NotificationRepository;

        public BlogController(IBlogRepository BlogRepository, ISiteRepository SiteRepository, ISettingRepository SettingRepository, ISubscriberRepository SubscriberRepository, INotificationRepository NotificationRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _BlogRepository = BlogRepository;
            _SiteRepository = SiteRepository;
            _SettingRepository = SettingRepository;
            _SubscriberRepository = SubscriberRepository;
            _NotificationRepository = NotificationRepository;
        }

        // GET: api/<controller>?moduleid=x&search=y
        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<Blog> Get(string moduleid, string search)
        {
            if (int.Parse(moduleid) == _authEntityId[EntityNames.Module])
            {
                return _BlogRepository.GetBlogs(int.Parse(moduleid), search);
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
            Blog Blog = _BlogRepository.GetBlog(id);
            if (Blog != null && Blog.ModuleId != _authEntityId[EntityNames.Module])
            {
                Blog = null;
            }
            return Blog;
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public Blog Post([FromBody] Blog Blog)
        {
            if (ModelState.IsValid && Blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                Blog = _BlogRepository.AddBlog(Blog);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Added {Blog}", Blog);
            }
            return Blog;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "EditModule")]
        public Blog Put(int id, [FromBody] Blog Blog)
        {
            if (ModelState.IsValid && Blog.BlogId == id && Blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                Blog = _BlogRepository.UpdateBlog(Blog);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Blog Updated {Blog}", Blog);
            }
            return Blog;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "EditModule")]
        public void Delete(int id)
        {
            Blog Blog = _BlogRepository.GetBlog(id);
            if (Blog != null && Blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                _BlogRepository.DeleteBlog(id);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Blog Deleted {BlogId}", id);
            }
        }

        [HttpGet("notify/{id}")]
        [Authorize(Policy = "EditModule")]
        public int Notify(int id)
        {
            var subscribers = 0;
            Blog Blog = _BlogRepository.GetBlog(id);
            if (Blog != null && Blog.ModuleId == _authEntityId[EntityNames.Module] && Blog.Published)
            {
                var settings = _SettingRepository.GetSettings(EntityNames.Module, Blog.ModuleId);
                if (settings.Any(item => item.SettingName == "Subscriptions") && settings.First(item => item.SettingName == "Subscriptions").SettingValue == "True")
                {
                    var alias = HttpContext.GetAlias();
                    var rooturl = alias.Protocol + alias.Name;
                    var pagepath = settings.First(item => item.SettingName == "PagePath").SettingValue;
                    var sender = settings.First(item => item.SettingName == "Sender").SettingValue;
                    var parameters = Utilities.AddUrlParameters(Blog.BlogId, Common.FormatSlug(Blog.Title));
                    var url = rooturl + Utilities.NavigateUrl(alias.Path, pagepath, parameters);

                    // this will likely need to be asynchronous in the future as it may timeout
                    foreach (var subscriber in _SubscriberRepository.GetSubscribers(Blog.ModuleId))
                    {
                        var body = Blog.Summary;
                        body += $"<br /><br />Read Full Article: <a href=\"{url}\">{url}</a>";
                        var unsubscribe = rooturl + Utilities.NavigateUrl(alias.Path, pagepath, "guid=" + subscriber.Guid);
                        body += $"<br /><br />Unsubscribe: <a href=\"{unsubscribe}\">{unsubscribe}</a>";
                        var notification = new Notification(alias.SiteId, "", sender, "", subscriber.Email, Blog.Title, body);
                        _NotificationRepository.AddNotification(notification);
                        subscribers++;
                    }
                }
            }
            return subscribers;
        }

        [HttpGet("rss/{id}")]
        public IActionResult RSS(int id)
        {
            var alias = HttpContext.GetAlias();
            var rooturl = alias.Protocol + alias.Name;

            var site = _SiteRepository.GetSite(alias.SiteId);
            var settings = _SettingRepository.GetSettings(EntityNames.Module, id);
            var pagepath = "/" + settings.First(item => item.SettingName == "PagePath").SettingValue;

            var rss = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" + Environment.NewLine;
            rss += "<rss version=\"2.0\">" + Environment.NewLine;
            rss += "<channel>" + Environment.NewLine;
            rss += "<title>" + WebUtility.HtmlEncode(site.Name) + "</title>" + Environment.NewLine;
            rss += "<link>" + rooturl + pagepath + "</link>" + Environment.NewLine;
            rss += "<description>" + WebUtility.HtmlEncode(site.Name) + "</description>" + Environment.NewLine;

            List<Blog> Blogs = _BlogRepository.GetBlogs(id, "").ToList();
            foreach (var Blog in Blogs.Where(item => item.Published))
            {
                rss += "<item>" + Environment.NewLine;
                rss += "<title>" + WebUtility.HtmlEncode(Blog.Title) + "</title>" + Environment.NewLine;
                var parameters = Utilities.AddUrlParameters(Blog.BlogId, Common.FormatSlug(Blog.Title));
                rss += "<link>" + rooturl + Utilities.NavigateUrl(alias.Path, pagepath, parameters)  + "</link>" + Environment.NewLine;
                rss += "<description>" + WebUtility.HtmlEncode(Blog.Summary) + "</description>" + Environment.NewLine;
                rss += "</item>" + Environment.NewLine;
            }

            rss += "</channel>" + Environment.NewLine;
            rss += "</rss>" + Environment.NewLine;

            return Content(rss, "application/xml");
        }
    }
}
