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
    public class BlogTagController : ModuleControllerBase
    {
        private readonly IBlogTagSourceRepository _blogTagSourceRepository;

        public BlogTagController(IBlogTagSourceRepository blogTagSourceRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _blogTagSourceRepository = blogTagSourceRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ViewModule")]
        public IEnumerable<BlogTagSource> Get(string moduleId)
        {
            if (int.Parse(moduleId) == _authEntityId[EntityNames.Module])
            {
                return _blogTagSourceRepository.GetBlogTagSources(int.Parse(moduleId));
            }
            else
            {
                return null;
            }
        }

    }
}
