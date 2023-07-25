using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class MailingCreditReport
    {
        public string Mail { get; set; }
        public int TotalCredit { get; set; }
        public int CreditInAmount { get; set; }

        public string Name { get; set; }
    }
}
