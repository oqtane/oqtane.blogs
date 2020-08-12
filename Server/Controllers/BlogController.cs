using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Blogs.Models;
using Oqtane.Blogs.Repository;
using Microsoft.AspNetCore.Http;

namespace Oqtane.Blogs.Controllers
{
    [Route("{site}/api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IBlogRepository _Blogs;
        private readonly ILogManager _logger;
        protected int _entityId = -1;

        public BlogController(IBlogRepository Blogs, ILogManager logger, IHttpContextAccessor accessor)
        {
            _Blogs = Blogs;
            _logger = logger;

            if (accessor.HttpContext.Request.Query.ContainsKey("entityid"))
            {
                _entityId = int.Parse(accessor.HttpContext.Request.Query["entityid"]);
            }
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<Blog> Get(string moduleid)
        {
            return _Blogs.GetBlogs(int.Parse(moduleid));
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize(Policy = "ViewModule")]
        public Blog Get(int id)
        {
            return _Blogs.GetBlog(id);
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public Blog Post([FromBody] Blog Blog)
        {
            if (ModelState.IsValid)
            {
                Blog = _Blogs.AddBlog(Blog);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Added {Blog}", Blog);
            }
            return Blog;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "EditModule")]
        public Blog Put(int id, [FromBody] Blog Blog)
        {
            if (ModelState.IsValid)
            {
                Blog = _Blogs.UpdateBlog(Blog);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Blog Updated {Blog}", Blog);
            }
            return Blog;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "EditModule")]
        public void Delete(int id)
        {
            _Blogs.DeleteBlog(id);
            _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Blog Deleted {BlogId}", id);
        }
    }
}
