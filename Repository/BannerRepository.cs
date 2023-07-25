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
    public class BannerRepository : RepositoryBase<Banner>, IBannerRepository
    {
        public BannerRepository(RepositoryContext repositoryContext)
           : base(repositoryContext)
        {
        }

        public IEnumerable<Banner> GetAllBanner()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<Banner> GetBanner(Expression<Func<Banner, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public Banner GetBannerById(int bannerId)
        {
            return FindByCondition(test => test.Id.Equals(bannerId))
                .FirstOrDefault();
        }

        public Banner CreateBanner(Banner banner)
        {
            try
            {
                Create(banner);
                return banner;
            }
            catch (Exception ex)
            {
                return banner;
            }
        }

        public bool UpdateBanner(Banner banner)
        {
            try
            {
                Update(banner);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteBanner(Banner banner)
        {
            try
            {
                Delete(banner);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
