using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class TimeZoneLocation
    {
        public string status { get; set; }
        public string message { get; set; }
        public string fromZoneName { get; set; }
        public string fromAbbreviation { get; set; }
        public int fromTimestamp { get; set; }
        public string toZoneName { get; set; }
        public string toAbbreviation { get; set; }
        public int toTimestamp { get; set; }
        public int offset { get; set; }
    }
}
