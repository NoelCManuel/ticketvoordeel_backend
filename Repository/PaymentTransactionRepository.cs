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
    public class PaymentTransactionRepository : RepositoryBase<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(RepositoryContext repositoryContext)
           : base(repositoryContext)
        {
        }

        public IEnumerable<PaymentTransaction> GetAllPaymentTransaction()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<PaymentTransaction> GetPaymentTransaction(Expression<Func<PaymentTransaction, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public PaymentTransaction GetPaymentTransactionById(int paymentTransactionId)
        {
            return FindByCondition(test => test.Id.Equals(paymentTransactionId))
                .FirstOrDefault();
        }

        public PaymentTransaction CreatePaymentTransaction(PaymentTransaction paymentTransaction)
        {
            try
            {
                Create(paymentTransaction);
                return paymentTransaction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdatePaymentTransaction(PaymentTransaction paymentTransaction)
        {
            try
            {
                Update(paymentTransaction);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeletePaymentTransaction(PaymentTransaction paymentTransaction)
        {
            try
            {
                Delete(paymentTransaction);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
