using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class PaymentHistory
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string ShopId { get; set; }
        public string PaymentId { get; set; }
        public string PurchaseId { get; set; }
        public string Amount { get; set; }
        public string IssuerId { get; set; }
        public bool TestMode { get; set; }
        public string EntanceCode { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaidDate { get; set; }
        public PaymentHistory()
        {
            PaidDate = DateTime.Now;
        }
    }
}
