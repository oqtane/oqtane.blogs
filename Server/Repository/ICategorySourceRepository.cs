using System.Collections.Generic;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public interface ICategorySourceRepository
    {
        IEnumerable<CategorySource> GetCategorySources(int moduleId);
        CategorySource AddCategorySource(CategorySource categorySource);
        CategorySource UpdateCategorySource(CategorySource categorySource);
        void DeleteCategorySource(int categorySourceId);
    }
}
