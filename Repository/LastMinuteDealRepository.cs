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
    public class LastMinuteDealRepository : RepositoryBase<LastMinuteDeal>, ILastMinuteDealsRepository
    {
        public LastMinuteDealRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<LastMinuteDeal> GetAllLastMinuteDeals()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<LastMinuteDeal> GetLastMinuteDeals(Expression<Func<LastMinuteDeal, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public LastMinuteDeal GetLastMinuteDealById(int id)
        {
            return FindByCondition(c => c.Id.Equals(id))
                .FirstOrDefault();
        }

        public LastMinuteDeal CreateLastMinuteDeal(LastMinuteDeal lastMinuteDeal)
        {
            try
            {
                Create(lastMinuteDeal);
                return lastMinuteDeal;
            }
            catch (Exception ex)
            {
                return lastMinuteDeal;
            }
        }

        public bool UpdateLastMinuteDeal(LastMinuteDeal lastMinuteDeal)
        {
            try
            {
                Update(lastMinuteDeal);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteLastMinuteDeal(LastMinuteDeal lastMinuteDeal)
        {
            try
            {
                Delete(lastMinuteDeal);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
