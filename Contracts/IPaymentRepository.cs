using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IPaymentRepository
    {
        IEnumerable<PaymentHistory> GetAllPayment();
        IEnumerable<PaymentHistory> GetPayment(Expression<Func<PaymentHistory, bool>> predicate);
        PaymentHistory GetPaymentById(int paymentId);
        PaymentHistory CreatePayment(PaymentHistory payment);
        bool UpdatePayment(PaymentHistory payment);
        bool DeletePayment(PaymentHistory payment);
    }
}
