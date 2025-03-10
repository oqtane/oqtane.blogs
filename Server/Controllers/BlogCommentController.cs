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
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using System.Net;
using Oqtane.Extensions;
using System.Linq;

namespace Oqtane.Blogs.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BlogCommentController : ModuleControllerBase
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogCommentRepository _blogCommentRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly Alias _alias;

        public BlogCommentController(IBlogRepository blogRepository, IBlogCommentRepository blogCommentRepository, INotificationRepository notificationRepository, ISettingRepository settingRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _blogRepository = blogRepository;
            _blogCommentRepository = blogCommentRepository;
            _notificationRepository = notificationRepository;
            _settingRepository = settingRepository;
            _userPermissions = userPermissions;
            _alias = tenantManager.GetAlias();
        }

        // GET api/<controller>?id=5&published=true
        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<BlogComment> Get(int id, bool published)
        {
            return _blogCommentRepository.GetBlogComments(id, _authEntityId[EntityNames.Module], published);
        }

        // GET api/<controller>/5/6
        [HttpGet("{id}/{blogId}")]
        [Authorize(Policy = "ViewModule")]
        public BlogComment Get(int id, int blogId)
        {
            BlogComment blogComment = null;
            var blog = _blogRepository.GetBlog(blogId);
            if (blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                blogComment = _blogCommentRepository.GetBlogComment(id);
                if (blogComment?.BlogId != blogId) blogComment = null;
            }
            return blogComment;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Authorize(Policy = "ViewModule")]
        public BlogComment Post([FromBody] BlogComment blogComment)
        {
            var blog = _blogRepository.GetBlog(blogComment.BlogId);
            if (ModelState.IsValid && blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                blogComment.Name = WebUtility.HtmlEncode(blogComment.Name);
                blogComment.Email = WebUtility.HtmlEncode(blogComment.Email);
                blogComment.Comment = WebUtility.HtmlEncode(blogComment.Comment).Replace("&#39;", "'");
                blogComment.IsPublished = _userPermissions.IsAuthorized(User, _alias.SiteId,EntityNames.Module, blog.ModuleId, PermissionNames.Edit);
                blogComment = _blogCommentRepository.AddBlogComment(blogComment);
                if (!blogComment.IsPublished)
                {
                    string url = _alias.Protocol + _alias.Name + ((blogComment.PagePath == "") ? "" : "/" + blogComment.PagePath) + "/!/" + blogComment.BlogId.ToString() + "/?comment=" + blogComment.BlogCommentId.ToString() + "&created=" + blogComment.CreatedOn.ToString("yyyyMMddHHmmssfff");
                    var body = "You Recently Submitted A Comment To The Blog \"" + blog.Title + "\". Please Use The Following Link To Publish Or Edit Your Comment: " + url;
                    var notification = new Notification(_alias.SiteId, blogComment.Name, blogComment.Email, "Blog Comment Authorization", body);
                    _notificationRepository.AddNotification(notification);

                    var settings = _settingRepository.GetSettings(EntityNames.Module, blog.ModuleId);
                    var sender = settings.FirstOrDefault(item => item.SettingName == "Sender");
                    if (sender != null)
                    {
                        url = _alias.Protocol + _alias.Name + ((blogComment.PagePath == "") ? "" : "/" + blogComment.PagePath);
                        body = "A Comment Was Recently Submitted To The Blog \"" + blog.Title + "\" By " + blogComment.Name;
                        body += "<br /><br />" + blogComment.Comment;
                        body += "<br /><br />Please Use The Following Link To Manage Comments: " + url;
                        notification = new Notification(_alias.SiteId, "Blog Administrator", sender.SettingValue, "Blog Comment Notification", body);
                        _notificationRepository.AddNotification(notification);
                    }
                }
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Comment Added {blogComment}", blogComment);
            }
            return blogComment;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [IgnoreAntiforgeryToken]
        [Authorize(Policy = "ViewModule")]
        public BlogComment Put(int id, [FromBody] BlogComment blogComment)
        {
            var blog = _blogRepository.GetBlog(blogComment.BlogId);
            if (ModelState.IsValid && blog.ModuleId == _authEntityId[EntityNames.Module] && blogComment.BlogCommentId == id)
            {
                var name = WebUtility.HtmlEncode(blogComment.Name);
                var email = WebUtility.HtmlEncode(blogComment.Email);
                var comment = WebUtility.HtmlEncode(blogComment.Comment);
                var published = blogComment.IsPublished;
                var created = blogComment.CreatedOn;

                blogComment = _blogCommentRepository.GetBlogComment(id);
                if (blogComment != null && blogComment.BlogId == blog.BlogId && blogComment.CreatedOn.ToString() == created.ToString())
                {
                    blogComment.Name = name;
                    blogComment.Email = email;
                    blogComment.Comment = comment;
                    blogComment.IsPublished = published;
                    blogComment = _blogCommentRepository.UpdateBlogComment(blogComment);
                    if (!blogComment.IsPublished)
                    {
                        string url = _alias.Protocol + _alias.Name + ((blogComment.PagePath == "") ? "" : "/" + blogComment.PagePath) + "/!/" + blogComment.BlogId.ToString() + "/?comment=" + blogComment.BlogCommentId.ToString() + "&created=" + blogComment.CreatedOn.ToString("yyyyMMddHHmmssfff");
                        var body = "You Recently Updated A Comment To The Blog: " + blog.Title + ". Please Use The Following Link To Publish Or Edit Your Comment: " + url;
                        var notification = new Notification(_alias.SiteId, blogComment.Name, blogComment.Email, "Blog Comment Authorization", body);
                        _notificationRepository.AddNotification(notification);

                        var settings = _settingRepository.GetSettings(EntityNames.Module, blog.ModuleId);
                        var sender = settings.FirstOrDefault(item => item.SettingName == "Sender");
                        if (sender != null)
                        {
                            url = _alias.Protocol + _alias.Name + ((blogComment.PagePath == "") ? "" : "/" + blogComment.PagePath);
                            body = "A Comment Was Recently Updated For The Blog " + blog.Title + " By " + blogComment.Name;
                            body += "<br /><br />" + blogComment.Comment;
                            body += "<br /><br />Please Use The Following Link To Manage Comments: " + url;
                            notification = new Notification(_alias.SiteId, "Blog Administrator", sender.SettingValue, "Blog Comment Notification", body);
                            _notificationRepository.AddNotification(notification);
                        }
                    }
                    _logger.Log(LogLevel.Information, this, LogFunction.Update, "Blog Comment Updated {BlogComment}", blogComment);
                }
            }
            return blogComment;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{blogCommentId}/{blogId}")]
        [Authorize(Policy = "EditModule")]
        public void Delete(int blogCommentId, int blogId)
        {
            var blog = _blogRepository.GetBlog(blogId);
            if (blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                var blogComment = _blogCommentRepository.GetBlogComment(blogCommentId);
                if (blogComment.BlogId == blog.BlogId)
                {
                    _blogCommentRepository.DeleteBlogComment(blogCommentId);
                    _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Blog Comment Deleted {BlogCommentId}", blogCommentId);
                }
            }
        }
    }
}
