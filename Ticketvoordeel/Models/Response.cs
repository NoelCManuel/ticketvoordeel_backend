using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class Response
    {
        public object Data { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }

        public string Others1 { get; set; }
        public string Others2 { get; set; }
    }
}
