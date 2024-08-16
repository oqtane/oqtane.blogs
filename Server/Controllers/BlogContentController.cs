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
using System.Linq;

namespace Oqtane.Blogs.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BlogContentController : ModuleControllerBase
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogContentRepository _blogContentRepository;

        public BlogContentController(IBlogRepository BlogRepository, IBlogContentRepository blogContentRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _blogRepository = BlogRepository;
            _blogContentRepository = blogContentRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<BlogContent> Get(int moduleId, int blogId)
        {
            if (moduleId == _authEntityId[EntityNames.Module])
            {
                return _blogContentRepository.GetBlogContents(blogId);
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
                    blogContent = _blogContentRepository.RestoreBlogContent(blogContent);
                    _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Content Restored {blogContent}", blogContent);
                }
                else
                {
                    blogContent = _blogContentRepository.AddBlogContent(blogContent);
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
                blogContent = _blogContentRepository.UpdateBlogContent(blogContent);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Blog Content Updated {blogContent}", blogContent);
            }
            return blogContent;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{blogId}/{blogContentId}")]
        [Authorize(Policy = "EditModule")]
        public void Delete(int moduleId, int blogId, int blogContentId)
        {
            var blogContents = _blogContentRepository.GetBlogContents(blogId);
            if (blogContents.Any(i => i.BlogContentId == blogContentId) && moduleId == _authEntityId[EntityNames.Module])
            {
                _blogContentRepository.DeleteBlogContent(blogContentId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Blog Content Deleted {BlogContentId}", blogContentId);
            }
        }
    }
}
