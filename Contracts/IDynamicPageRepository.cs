using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IDynamicPageRepository
    {
        IEnumerable<DynamicPage> GetAllDynamicPages();
        IEnumerable<DynamicPage> GetDynamicPages(Expression<Func<DynamicPage, bool>> predicate);
        DynamicPage GetDynamicPageById(int dynamicPageId);
        DynamicPage CreateDynamicPage(DynamicPage dynamicPage);
        bool UpdateDynamicPage(DynamicPage dynamicPage);
        bool DeleteDynamicPage(DynamicPage dynamicPage);
    }
}
