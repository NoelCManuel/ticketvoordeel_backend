using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class LastMinuteDealsResponse
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int ColumnNumber { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string Airline { get; set; }
        public LastMinuteDeals PoolRequest { get; set; }
    }

    public class LastMinuteDealsRequest
    { 
        public int Id { get; set; }
        public DateTime TravelDate { get; set; }
        public string FromAirport { get; set; }
        public string ToAirport { get; set; }
        public string FromAirportCode { get; set; }
        public string ToAirportCode { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string Airline { get; set; }
        public decimal Price { get; set; }
        public int ColumnNumber { get; set; }
    }
}
