using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class PaymentCollection
    {
        public string BookingPnr { get; set; }
        public string PosCode { get; set; } = "SisowMC";
        public string OrderNo { get; set; } = "SISOW";
        public string Remark { get; set; } = DateTime.Now.ToString();
        public int TransactionType { get; set; } = 1;
        public string CurrencyCode { get; set; } = "EUR";
        public string AccountOwner { get; set; } = "NA";
        public decimal PaymentAmount { get; set; }
    }
}
