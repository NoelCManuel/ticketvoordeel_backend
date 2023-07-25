using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class Origin
    {
        public string Code { get; set; }
        public bool IsCity { get; set; }

    }

    public class Destination
    {
        public string Code { get; set; }
        public bool IsCity { get; set; }

    }

    public class Departure
    {
        public string Date { get; set; }
        public int DaysBefore { get; set; } = 0;
        public int DaysAfter { get; set; } = 0;

    }

    public class FlightType
    {
        public int? MaxConnections { get; set; } = 0;
        public int? ConnectionType { get; set; }

    }

    public class Route
    {
        public Origin Origin { get; set; }
        public Destination Destination { get; set; }
        public Departure Departure { get; set; }
        public FlightType FlightType { get; set; }

    }

    public class Passengers
    {
        public int Adult { get; set; }
        public int Child { get; set; }
        public int Infant { get; set; }

    }

    public class Preference
    {
        public List<string> RequiredCarrierCodes { get; set; }
        public string CabinType { get; set; }

    }

    public class PoolRequest
    {
        public List<Route> Routes { get; set; }
        public Passengers Passengers { get; set; }
        public Preference Preference { get; set; }
    }

    public class PoolRequestBaseViewModel
    {
        public PoolRequest PoolRequest { get; set; }
        public string CurrencyCode { get; set; }

    }

    public class LastMinuteDealsRoot
    {
        public LastMinuteDeals LastMinuteDeals { get; set; }
        public OtherInfo OtherInfo { get; set; }
    }

    public class LastMinuteDeals
    {       
        public PoolRequest PoolRequest { get; set; }
        public string CurrencyCode { get; set; }       
    }

    public class OtherInfo
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
    }
}
