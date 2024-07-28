using System.Collections.Generic;
using System.Threading.Tasks;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Services
{
    public interface ICategoryService
    {
        Task<List<CategorySource>> GetCategorySourcesAsync(int ModuleId);

        Task<CategorySource> AddCategorySourceAsync(CategorySource categorySource);

        Task<CategorySource> UpdateCategorySourceAsync(CategorySource categorySource);

        Task DeleteCategorySourceAsync(int categorySourceId, int ModuleId);
    }
}
