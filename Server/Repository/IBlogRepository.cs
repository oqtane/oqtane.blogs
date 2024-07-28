using System.Collections.Generic;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public interface IBlogRepository
    {
        IEnumerable<Blog> GetBlogs(int moduleId, BlogSearch searchQuery);
        Blog GetBlog(int blogId);
        Blog GetBlogBySlug(string slug);
        Blog AddBlog(Blog blog);
        Blog UpdateBlog(Blog blog);
        void DeleteBlog(int blogId);

        IEnumerable<BlogContent> GetBlogContents(int blogId);
        BlogContent AddBlogContent(BlogContent blogContent);
        BlogContent UpdateBlogContent(BlogContent blogContent);
        BlogContent RestoreBlogContent(BlogContent blogContent);
        void DeleteBlogContent(int blogContentId);

    }
}
