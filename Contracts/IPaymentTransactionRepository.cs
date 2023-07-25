using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IPaymentTransactionRepository
    {
        IEnumerable<PaymentTransaction> GetAllPaymentTransaction();
        IEnumerable<PaymentTransaction> GetPaymentTransaction(Expression<Func<PaymentTransaction, bool>> predicate);
        PaymentTransaction GetPaymentTransactionById(int paymentTransactionId);
        PaymentTransaction CreatePaymentTransaction(PaymentTransaction paymentTransaction);
        bool UpdatePaymentTransaction(PaymentTransaction paymentTransaction);
        bool DeletePaymentTransaction(PaymentTransaction paymentTransaction);
    }
}