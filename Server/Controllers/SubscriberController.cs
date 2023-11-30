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
    public class SubscriberController : ModuleControllerBase
    {
        private readonly ISubscriberRepository _SubscriberRepository;

        public SubscriberController(ISubscriberRepository SubscriberRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _SubscriberRepository = SubscriberRepository;
        }

        // POST api/<controller>/5
        [HttpPost]
        public void Post([FromBody] Subscriber Subscriber)
        {
            if (ModelState.IsValid && Utilities.IsValidEmail(Subscriber.Email))
            {
                Subscriber.Guid = Guid.NewGuid().ToString();
                Subscriber = _SubscriberRepository.AddSubscriber(Subscriber);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Subscriber Added {Subscriber}", Subscriber);
            }
        }

        // DELETE api/<controller>/5/guid
        [HttpDelete("{moduleid}/{guid}")]
        public void Delete(int moduleid, string guid)
        {
            Subscriber Subscriber = _SubscriberRepository.GetSubscribers(moduleid).FirstOrDefault(item => item.Guid == guid);
            if (Subscriber != null)
            {
                _SubscriberRepository.DeleteSubscriber(Subscriber.SubscriberId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Subscriber Deleted {Subscriber}", Subscriber);
            }
        }
    }
}
