using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class InvoiceRequest
    {
        public string Pnr { get; set; }
        public string RelationId { get; set; }
        public int Type { get; set; }
        public int PriceType { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string ExtraRemarks { get; set; }
        public bool IsPDF { get; set; }
    }
}
