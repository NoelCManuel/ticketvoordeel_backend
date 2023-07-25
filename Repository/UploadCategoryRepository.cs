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
    public class UploadCategoryRepository : RepositoryBase<UploadCategory>, IUploadCategoryRepository
    {
        public UploadCategoryRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<UploadCategory> GetAllUploadCategories()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<UploadCategory> GetUploadCategory(Expression<Func<UploadCategory, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public UploadCategory GetUploadCategoryById(int id)
        {
            return FindByCondition(c => c.Id.Equals(id))
                .FirstOrDefault();
        }

        public UploadCategory CreateUploadCategory(UploadCategory uploadCategory)
        {
            try
            {
                Create(uploadCategory);
                return uploadCategory;
            }
            catch (Exception ex)
            {
                return uploadCategory;
            }
        }

        public bool UpdateUploadCategory(UploadCategory uploadCategory)
        {
            try
            {
                Update(uploadCategory);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteUploadCategory(UploadCategory uploadCategory)
        {
            try
            {
                Delete(uploadCategory);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
