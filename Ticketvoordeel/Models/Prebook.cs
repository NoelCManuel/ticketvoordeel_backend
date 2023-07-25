using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class PrebookRequest
    {
        public string BasketKey { get; set; }
    }

    public class PrebookResponse
    {
        public string ReferenceNumber { get; set; }
    }

    public class FinalBookResponse
    { 
        public string Pnr { get; set; }
        public bool HasProblem { get; set; }
        public string Message { get; set; }
    }
}
