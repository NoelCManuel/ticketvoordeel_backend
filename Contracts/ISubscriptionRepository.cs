using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;


namespace Contracts
{
    public interface ISubscriptionRepository
    {
        IEnumerable<Subscription> GetAllSubscription();
        Subscription GetSubscriptionById(int subscriptionId);
        Subscription CreateSubscription(Subscription subscription);
        bool UpdateSubscription(Subscription subscription);
        bool DeleteSubscription(Subscription subscription);
    }
}
