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
using System.Reflection;

namespace Oqtane.Blogs.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BlogContentController : ModuleControllerBase
    {
        private readonly IBlogRepository _blogRepository;

        public BlogContentController(IBlogRepository BlogRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _blogRepository = BlogRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<BlogContent> Get(int moduleId, int blogId)
        {
            if (moduleId == _authEntityId[EntityNames.Module])
            {
                return _blogRepository.GetBlogContents(blogId);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public BlogContent Post(int moduleId, string type, [FromBody] BlogContent blogContent)
        {
            if (ModelState.IsValid && moduleId == _authEntityId[EntityNames.Module])
            {
                if(type == "restore")
                {
                    blogContent = _blogRepository.RestoreBlogContent(blogContent);
                    _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Content Restored {blogContent}", blogContent);
                }
                else
                {
                    blogContent = _blogRepository.AddBlogContent(blogContent);
                    _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Content Added {blogContent}", blogContent);
                }
            }

            return blogContent;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "EditModule")]
        public BlogContent Put(int moduleId, int id, [FromBody] BlogContent blogContent)
        {
            if (ModelState.IsValid && blogContent.BlogContentId == id && moduleId == _authEntityId[EntityNames.Module])
            {
                blogContent = _blogRepository.UpdateBlogContent(blogContent);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Blog Content Updated {blogContent}", blogContent);
            }
            return blogContent;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{blogId}/{blogContentId}")]
        [Authorize(Policy = "EditModule")]
        public void Delete(int moduleId, int blogId, int blogContentId)
        {
            var blogContents = _blogRepository.GetBlogContents(blogId);
            if (blogContents.Any(i => i.BlogContentId == blogContentId) && moduleId == _authEntityId[EntityNames.Module])
            {
                _blogRepository.DeleteBlogContent(blogContentId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Blog Content Deleted {BlogContentId}", blogContentId);
            }
        }
    }
}
