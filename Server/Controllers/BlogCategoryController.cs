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
    public class BlogCategoryController : ModuleControllerBase
    {
        private readonly IBlogCategorySourceRepository _blogCategorySourceRepository;

        public BlogCategoryController(IBlogCategorySourceRepository blogCategorySourceRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _blogCategorySourceRepository = blogCategorySourceRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<BlogCategorySource> Get(string moduleId)
        {
            if (int.Parse(moduleId) == _authEntityId[EntityNames.Module])
            {
                return _blogCategorySourceRepository.GetBlogCategorySources(int.Parse(moduleId));
            }
            else
            {
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public BlogCategorySource Post([FromBody] BlogCategorySource blogCategorySource)
        {
            if (ModelState.IsValid && blogCategorySource.ModuleId == _authEntityId[EntityNames.Module])
            {
                blogCategorySource = _blogCategorySourceRepository.AddBlogCategorySource(blogCategorySource);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Blog Category Added {categorySource}", blogCategorySource);
            }
            return blogCategorySource;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "EditModule")]
        public BlogCategorySource Put(int id, [FromBody] BlogCategorySource blogCategorySource)
        {
            if (ModelState.IsValid && blogCategorySource.BlogCategorySourceId == id && blogCategorySource.ModuleId == _authEntityId[EntityNames.Module])
            {
                blogCategorySource = _blogCategorySourceRepository.UpdateBlogCategorySource(blogCategorySource);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Blog Category Updated {categorySource}", blogCategorySource);
            }
            return blogCategorySource;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "EditModule")]
        public void Delete(int id)
        {
            var blogCategorySource = _blogCategorySourceRepository.GetBlogCategorySources(_authEntityId[EntityNames.Module]).FirstOrDefault(i => i.BlogCategorySourceId == id);
            if (blogCategorySource != null && blogCategorySource.ModuleId == _authEntityId[EntityNames.Module])
            {
                _blogCategorySourceRepository.DeleteBlogCategorySource(id);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Blog Category Deleted {CategorySourceId}", id);
            }
        }        
    }
}
