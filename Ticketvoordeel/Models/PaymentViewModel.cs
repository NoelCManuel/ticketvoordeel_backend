using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketvoordeel.Models.TravelInfo;

namespace Ticketvoordeel.Models
{
    public class SisowIssuer
    {
        public string IssueId { get; set; }

        public string IssuerName { get; set; }
    }

    public class PaymentURLRequest
    { 
        public string paymentType { get; set; }
        public string issuerId { get; set; }
        public double amount { get; set; }
        public string paymentMethod { get; set; }
        public BookRequest.BookRequest bookRequest { get; set; }            
    }

    public class PaymentURLResponse
    {
        public string PaymentURL { get; set; }
        public string TransactionId { get; set; }
    }
}
