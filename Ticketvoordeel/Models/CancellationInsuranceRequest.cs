using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class CancellationInsuranceRequest
    {
        public string StartDate { get; set; }
        public decimal Amount { get; set; }
    }
}
