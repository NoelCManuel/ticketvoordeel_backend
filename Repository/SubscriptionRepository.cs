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
    public class SubscriptionRepository : RepositoryBase<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Subscription> GetAllSubscription()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public Subscription GetSubscriptionById(int testId)
        {
            return FindByCondition(test => test.Id.Equals(testId))
                .FirstOrDefault();
        }

        public Subscription CreateSubscription(Subscription subscription)
        {
            try
            {
                Create(subscription);
                return subscription;
            }
            catch (Exception ex)
            {
                return subscription;
            }
        }

        public bool UpdateSubscription(Subscription subscription)
        {
            try
            {
                Update(subscription);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteSubscription(Subscription subscription)
        {
            try
            {
                Delete(subscription);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
