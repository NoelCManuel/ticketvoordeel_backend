using System;

namespace Entities.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CustomerName { get; set; }
        public bool IsRoundTrip { get; set; }
        public string DeparturePnr { get; set; }
        public string ReturnPnr { get; set; }
        public string BookingDetails { get; set; }
        public DateTime CreationTime { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public int CreditReceived { get; set; }
        public int CreditApplied { get; set; } = 0;
        public string TransactionId { get; set; }
        public int ServicePackageAmount { get; set; }
        public string UserEmail { get; set; }
        public string SuvendusApplied { get; set; }
        public Booking()
        {
            CreationTime = DateTime.Now;
        }
    }
}
