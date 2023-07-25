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
    public class DynamicPageRepository : RepositoryBase<DynamicPage>, IDynamicPageRepository
    {
        public DynamicPageRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<DynamicPage> GetAllDynamicPages()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<DynamicPage> GetDynamicPages(Expression<Func<DynamicPage, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public DynamicPage GetDynamicPageById(int id)
        {
            return FindByCondition(c => c.Id.Equals(id))
                .FirstOrDefault();
        }

        public DynamicPage CreateDynamicPage(DynamicPage dynamicPage)
        {
            try
            {
                Create(dynamicPage);
                return dynamicPage;
            }
            catch (Exception ex)
            {
                return dynamicPage;
            }
        }

        public bool UpdateDynamicPage(DynamicPage dynamicPage)
        {
            try
            {
                Update(dynamicPage);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }

        public bool DeleteDynamicPage(DynamicPage dynamicPage)
        {
            try
            {
                Delete(dynamicPage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
