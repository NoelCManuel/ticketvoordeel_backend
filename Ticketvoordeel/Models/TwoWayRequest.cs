using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.TwoWayRequest
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
        public int DaysBefore { get; set; }
    }

    public class FlightType
    {
        public int MaxConnections { get; set; }
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
        public string RequiredCarrierCodes { get; set; }
        public string CabinType { get; set; }
        public List<SelectedFlights> SelectedFlights { get; set; }

    }

    public class SelectedFlights
    {
        public string FlightNumber { get; set; }
        public string FareClass { get; set; }
    }

    public class PoolRequest
    {
        public List<Route> Routes { get; set; }
        public Passengers Passengers { get; set; }
        public Preference Preference { get; set; }
    }

    public class TwoWayRequest
    {
        public PoolRequest PoolRequest { get; set; }
        public string CurrencyCode { get; set; }
        public SelectedAirlines SelectedAirlines { get; set; }
    }

    public class SelectedAirlines
    { 
        public List<SelectedAirlineDetail> DepartureAirlines { get; set; }
        public List<SelectedAirlineDetail> ReturnAirlines { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class SelectedAirlineDetail
    {
        public string AirlineCode { get; set; }
        public string AirlineClass { get; set; }
    }
}
