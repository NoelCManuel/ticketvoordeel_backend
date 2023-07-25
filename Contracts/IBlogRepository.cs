using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IBlogRepository
    {
        IEnumerable<Blog> GetAllBlog();
        IEnumerable<Blog> GetBlog(Expression<Func<Blog, bool>> predicate);
        Blog GetBlogById(int blogId);
        Blog CreateBlog(Blog blog);
        bool UpdateBlog(Blog blog);
        bool DeleteBlog(Blog blog);
    }
}
