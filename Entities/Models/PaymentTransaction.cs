using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public string BookRequest { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string DateTime { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
    }
}
