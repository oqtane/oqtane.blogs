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
    public class CategoryController : ModuleControllerBase
    {
        private readonly ICategorySourceRepository _categorySourceRepository;

        public CategoryController(ICategorySourceRepository categorySourceRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _categorySourceRepository = categorySourceRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<CategorySource> Get(string moduleId)
        {
            if (int.Parse(moduleId) == _authEntityId[EntityNames.Module])
            {
                return _categorySourceRepository.GetCategorySources(int.Parse(moduleId));
            }
            else
            {
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = "EditModule")]
        public CategorySource Post([FromBody] CategorySource categorySource)
        {
            if (ModelState.IsValid && categorySource.ModuleId == _authEntityId[EntityNames.Module])
            {
                categorySource = _categorySourceRepository.AddCategorySource(categorySource);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Categrory Added {categorySource}", categorySource);
            }
            return categorySource;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "EditModule")]
        public CategorySource Put(int id, [FromBody] CategorySource categorySource)
        {
            if (ModelState.IsValid && categorySource.CategorySourceId == id && categorySource.ModuleId == _authEntityId[EntityNames.Module])
            {
                categorySource = _categorySourceRepository.UpdateCategorySource(categorySource);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Categrory Updated {categorySource}", categorySource);
            }
            return categorySource;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "EditModule")]
        public void Delete(int id)
        {
            var categorySource = _categorySourceRepository.GetCategorySources(_authEntityId[EntityNames.Module]).FirstOrDefault(i => i.CategorySourceId == id);
            if (categorySource != null && categorySource.ModuleId == _authEntityId[EntityNames.Module])
            {
                _categorySourceRepository.DeleteCategorySource(id);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Categrory Deleted {CategorySourceId}", id);
            }
        }        
    }
}
