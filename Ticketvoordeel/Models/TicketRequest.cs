using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class TicketRequest
    {
        public string pnr { get; set; }
        public string toEmail { get; set; }
        public string subject { get; set; }
        public int type { get; set; }
    }
}
