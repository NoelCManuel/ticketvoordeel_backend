using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface ILastMinuteDealsRepository
    {
        IEnumerable<LastMinuteDeal> GetAllLastMinuteDeals();
        IEnumerable<LastMinuteDeal> GetLastMinuteDeals(Expression<Func<LastMinuteDeal, bool>> predicate);
        LastMinuteDeal GetLastMinuteDealById(int lastMinuteDealId);
        LastMinuteDeal CreateLastMinuteDeal(LastMinuteDeal lastMinuteDeal);
        bool UpdateLastMinuteDeal(LastMinuteDeal lastMinuteDeal);
        bool DeleteLastMinuteDeal(LastMinuteDeal lastMinuteDeal);
    }
}
