using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository
{
    public class BlogRepository : RepositoryBase<Blog>, IBlogRepository
    {
        public BlogRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Blog> GetAllBlog()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<Blog> GetBlog(Expression<Func<Blog, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public Blog GetBlogById(int blogId)
        {
            return FindByCondition(test => test.Id.Equals(blogId))
                .FirstOrDefault();
        }

        public Blog CreateBlog(Blog blog)
        {
            try
            {
                Create(blog);
                return blog;
            }
            catch (Exception ex)
            {
                return blog;
            }
        }

        public bool UpdateBlog(Blog blog)
        {
            try
            {
                Update(blog);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteBlog(Blog blog)
        {
            try
            {
                Delete(blog);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
