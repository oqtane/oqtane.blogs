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
using System.Threading.Tasks;

namespace Oqtane.Blogs.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BlogController : ModuleControllerBase
    {
        private readonly IBlogRepository _BlogRepository;

        public BlogController(IBlogRepository BlogRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger,accessor)
        {
            _BlogRepository = BlogRepository;
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public async Task<IEnumerable<Blog>> Get(string moduleid)
        {
            if (int.Parse(moduleid) == _authEntityId[EntityNames.Module])
            {
                return await _BlogRepository.GetBlogsAsync(int.Parse(moduleid), false);
            }
            else
            {
                return null;
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize(Policy = "ViewModule")]
        public async Task<Blog> Get(int id)
        {
            Blog Blog = await _BlogRepository.GetBlogAsync(id);
            if (Blog != null && Blog.ModuleId != _authEntityId[EntityNames.Module])
            {
                Blog = null;
            }
            return Blog;
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public async Task<Blog> Post([FromBody] Blog Blog)
        {
            if (ModelState.IsValid && Blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                Blog = await _BlogRepository.AddBlogAsync(Blog);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Added {Blog}", Blog);
            }
            return Blog;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "EditModule")]
        public async Task<Blog> Put(int id, [FromBody] Blog Blog)
        {
            if (ModelState.IsValid && Blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                Blog = await _BlogRepository.UpdateBlogAsync(Blog);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Blog Updated {Blog}", Blog);
            }
            return Blog;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "EditModule")]
        public async Task Delete(int id)
        {
            Models.Blog Blog = await _BlogRepository.GetBlogAsync(id);
            if (Blog != null && Blog.ModuleId == _authEntityId[EntityNames.Module])
            {
                await _BlogRepository.DeleteBlogAsync(id);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Blog Deleted {BlogId}", id);
            }
        }
    }
}
