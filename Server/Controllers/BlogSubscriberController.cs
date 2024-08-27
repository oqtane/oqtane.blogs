using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Blogs.Models;
using Oqtane.Blogs.Repository;
using Microsoft.AspNetCore.Http;
using Oqtane.Controllers;
using System.Linq;
using System;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Extensions;

namespace Oqtane.Blogs.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BlogSubscriberController : ModuleControllerBase
    {
        private readonly IBlogSubscriberRepository _BlogSubscriberRepository;
        private readonly ISettingRepository _SettingRepository;
        private readonly INotificationRepository _NotificationRepository;

        public BlogSubscriberController(IBlogSubscriberRepository BlogSubscriberRepository, ISettingRepository SettingRepository, INotificationRepository NotificationRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _BlogSubscriberRepository = BlogSubscriberRepository;
            _SettingRepository = SettingRepository;
            _NotificationRepository = NotificationRepository;
        }

        // POST api/<controller>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public void Post([FromBody] BlogSubscriber BlogSubscriber)
        {
            if (ModelState.IsValid && Utilities.IsValidEmail(BlogSubscriber.Email))
            {
                BlogSubscriber.IsVerified = false;
                BlogSubscriber.Guid = Guid.NewGuid().ToString();
                BlogSubscriber = _BlogSubscriberRepository.AddBlogSubscriber(BlogSubscriber);

                if (BlogSubscriber != null)
                {
                    _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Subscriber Added {BlogSubscriber}", BlogSubscriber);

                    var settings = _SettingRepository.GetSettings(EntityNames.Module, BlogSubscriber.ModuleId);
                    var alias = HttpContext.GetAlias();
                    var pagepath = settings.First(item => item.SettingName == "PagePath").SettingValue;
                    var sender = settings.First(item => item.SettingName == "Sender").SettingValue;
                    var body = "Thank You For Subscribing To Our Blog. Please Verify Your Email Address By Clicking The Link Below.";
                    var url = alias.Protocol + alias.Name + Utilities.NavigateUrl(alias.Path, pagepath, "guid=" + BlogSubscriber.Guid + "&action=verify");
                    body += $"<br /><br />Verify: <a href=\"{url}\">{url}</a>";
                    var notification = new Notification(alias.SiteId, "", sender, "", BlogSubscriber.Email, "Blog Subscription", body);
                    _NotificationRepository.AddNotification(notification);
                }
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{moduleid}/{guid}")]
        [IgnoreAntiforgeryToken]
        public void Put(int moduleid, string guid)
        {
            BlogSubscriber BlogSubscriber = _BlogSubscriberRepository.GetBlogSubscribers(moduleid).FirstOrDefault(item => item.Guid == guid);
            if (BlogSubscriber != null)
            {
                BlogSubscriber.IsVerified = true;
                BlogSubscriber = _BlogSubscriberRepository.UpdateBlogSubscriber(BlogSubscriber);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Blog Subscriber Updated {BlogSubscriber}", BlogSubscriber);
            }
        }
        
        // DELETE api/<controller>/5/guid
        [HttpDelete("{moduleid}/{guid}")]
        [IgnoreAntiforgeryToken]
        public void Delete(int moduleid, string guid)
        {
            BlogSubscriber BlogSubscriber = _BlogSubscriberRepository.GetBlogSubscribers(moduleid).FirstOrDefault(item => item.Guid == guid);
            if (BlogSubscriber != null)
            {
                _BlogSubscriberRepository.DeleteBlogSubscriber(BlogSubscriber.BlogSubscriberId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Blog Subscriber Deleted {BlogSubscriber}", BlogSubscriber);
            }
        }
    }
}
