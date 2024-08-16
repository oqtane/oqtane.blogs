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

namespace Oqtane.Blogs.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BlogSubscriberController : ModuleControllerBase
    {
        private readonly IBlogSubscriberRepository _BlogSubscriberRepository;

        public BlogSubscriberController(IBlogSubscriberRepository BlogSubscriberRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _BlogSubscriberRepository = BlogSubscriberRepository;
        }

        // POST api/<controller>/5
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public void Post([FromBody] BlogSubscriber BlogSubscriber)
        {
            if (ModelState.IsValid && Utilities.IsValidEmail(BlogSubscriber.Email))
            {
                BlogSubscriber.Guid = Guid.NewGuid().ToString();
                BlogSubscriber = _BlogSubscriberRepository.AddBlogSubscriber(BlogSubscriber);
                if (BlogSubscriber != null)
                {
                    _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Subscriber Added {BlogSubscriber}", BlogSubscriber);
                }
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
