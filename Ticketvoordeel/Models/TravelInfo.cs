using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.TravelInfo
{
    public class OriginAirport
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public object CityName { get; set; }
        public bool Deleted { get; set; }
    }

    public class DestinationAirport
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public object CityName { get; set; }
        public bool Deleted { get; set; }
    }

    public class MarketingCarrier
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Carrier
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Origin
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        //public object CityName { get; set; }
        //public bool Deleted { get; set; }
    }

    public class Destination
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        //public object CityName { get; set; }
        //public bool Deleted { get; set; }
    }

    public class Leg
    {
        public Origin Origin { get; set; }
        public Destination Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        //public object Aircraft { get; set; }
        //public object OriginTerminal { get; set; }
        //public object DestinationTerminal { get; set; }
        //public object Duration { get; set; }
        //public bool ChangePlane { get; set; }
    }

    public class Segment
    {
        public Carrier Carrier { get; set; }
        //public object OperatingCarrier { get; set; }
        public int Duration { get; set; }
        public string Number { get; set; }
        public List<Leg> Legs { get; set; }
        public string BookingClass { get; set; }
        //public object Remark { get; set; }
        //public int Stopover { get; set; }
    }

    public class TotalPrice
    {
        public string Currency { get; set; }
        public double Base { get; set; }
        public double Tax { get; set; }
        public int ServiceFee { get; set; }
        public int AgencyFee { get; set; }
        public int CompanyFee { get; set; }
        public double TotalFeeless { get; set; }
        public double Total { get; set; }
        public object Internal { get; set; }
    }

    public class Adult
    {
        public string Currency { get; set; }
        public double Base { get; set; }
        public double Tax { get; set; }
        public int ServiceFee { get; set; }
        public int AgencyFee { get; set; }
        public int CompanyFee { get; set; }
        public double TotalFeeless { get; set; }
        public double Total { get; set; }
        public object Internal { get; set; }
    }

    public class PassengerPrice
    {
        public Adult Adult { get; set; }
    }

    public class FreeBaggage
    {
        public int Amount { get; set; }
        public int Unit { get; set; }
        public object Pieces { get; set; }
    }

    public class Route
    {
        public OriginAirport OriginAirport { get; set; }
        public DestinationAirport DestinationAirport { get; set; }
        public MarketingCarrier MarketingCarrier { get; set; }
        public List<Segment> Segments { get; set; }
        //public bool HasMultiCarrier { get; set; }
        //public bool Visible { get; set; }
        //public TotalPrice TotalPrice { get; set; }
        //public PassengerPrice PassengerPrice { get; set; }
        //public string Id { get; set; }
        //public string IdStr { get; set; }
        //public string RouteFareOptionIdStr { get; set; }
        //public string RouteFareOptionId { get; set; }
        //public int Duration { get; set; }
        //public string ProductCode { get; set; }
        //public string ProviderCode { get; set; }
        //public FreeBaggage FreeBaggage { get; set; }
        //public object FreeCabinBaggage { get; set; }
        //public string FareClass { get; set; }
        //public int CabinClassType { get; set; }
        //public int SeatCount { get; set; }
        //public bool Alternative { get; set; }
        //public object AvailableServices { get; set; }
        //public string CabinClassName { get; set; }
        //public int Stopovers { get; set; }
    }

    public class Departure
    {
        public string FGId { get; set; }
        public string routeId { get; set; }
        public Route route { get; set; }
    }

    public class OperatingCarrier
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class FreeCabinBaggage
    {
        public int Amount { get; set; }
        public int Unit { get; set; }
        public object Pieces { get; set; }
    }

    public class Arrival
    {
        public string FGId { get; set; }
        public string routeId { get; set; }
        public Route route { get; set; }
    }

    public class TravelInfo
    {
        public Departure Departure { get; set; }
        public Arrival Arrival { get; set; }
    }
}
