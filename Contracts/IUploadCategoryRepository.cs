using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IUploadCategoryRepository
    {
        IEnumerable<UploadCategory> GetAllUploadCategories();
        IEnumerable<UploadCategory> GetUploadCategory(Expression<Func<UploadCategory, bool>> predicate);
        UploadCategory GetUploadCategoryById(int uploadCategoryId);
        UploadCategory CreateUploadCategory(UploadCategory uploadCategory);
        bool UpdateUploadCategory(UploadCategory uploadCategory);
        bool DeleteUploadCategory(UploadCategory uploadCategory);
    }
}
