using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IBannerRepository
    {
        IEnumerable<Banner> GetAllBanner();
        IEnumerable<Banner> GetBanner(Expression<Func<Banner, bool>> predicate);
        Banner GetBannerById(int bannerId);
        Banner CreateBanner(Banner banner);
        bool UpdateBanner(Banner banner);
        bool DeleteBanner(Banner banner);
    }
}
