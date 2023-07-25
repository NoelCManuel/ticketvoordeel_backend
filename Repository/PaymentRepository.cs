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
    public class PaymentRepository : RepositoryBase<PaymentHistory>, IPaymentRepository
    {
        public PaymentRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<PaymentHistory> GetAllPayment()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<PaymentHistory> GetPayment(Expression<Func<PaymentHistory, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public PaymentHistory GetPaymentById(int paymentId)
        {
            return FindByCondition(test => test.Id.Equals(paymentId))
                .FirstOrDefault();
        }

        public PaymentHistory CreatePayment(PaymentHistory payment)
        {
            try
            {
                Create(payment);
                return payment;
            }
            catch (Exception ex)
            {
                return payment;
            }
        }

        public bool UpdatePayment(PaymentHistory payment)
        {
            try
            {
                Update(payment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeletePayment(PaymentHistory payment)
        {
            try
            {
                Delete(payment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
