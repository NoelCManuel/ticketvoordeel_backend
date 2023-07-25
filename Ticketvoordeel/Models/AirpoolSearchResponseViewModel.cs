using System;
using System.Collections.Generic;

namespace Ticketvoordeel.Models.AirpoolSearchResponse
{
    public class AirpoolSearchResponseViewModel
    {
        public Reader Reader { get; set; }
    }

    public partial class Reader
    {
        public Currency Currency { get; set; }
        public Guid SearchId { get; set; }
        public long TripType { get; set; }
        public long ResultType { get; set; }
        public bool ExcludeServiceFee { get; set; }
        public bool UseTotalPrice { get; set; }
        public PassengerCounts PassengerCounts { get; set; }
        public Dictionary<string, City> Cities { get; set; }
        public long RouteDuration { get; set; }
        public List<FareGroup> FareGroups { get; set; }
        public bool ContainsAlternatives { get; set; }
        public long TotalGroupCount { get; set; }
        public long PageCount { get; set; }
        public long CurrentPage { get; set; }
        public object Routes { get; set; }
    }

    public partial class City
    {
        public long Code { get; set; }
        public string Name { get; set; }
        public string CodeIata { get; set; }
        public string CountryCode { get; set; }
    }

    public partial class FareGroup
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
        public bool InPage { get; set; }
        public bool IsMultiTicket { get; set; }
        public bool HasMultiCarrier { get; set; }
        public List<List<Route>> Routes { get; set; }
        public TotalPrice TotalPrice { get; set; }
        public PassengerPrice PassengerPrice { get; set; }
    }

    public partial class PassengerPrice
    {
        public TotalPrice Adult { get; set; }
    }

    public class MarketingCarrier
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public partial class TotalPrice
    {
        public Currency Currency { get; set; }
        public long Base { get; set; }
        public double Tax { get; set; }
        public long ServiceFee { get; set; }
        public long AgencyFee { get; set; }
        public long CompanyFee { get; set; }
        public double TotalFeeless { get; set; }
        public decimal Total { get; set; }
        public object Internal { get; set; }
    }

    public class Airport
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
    }

    public partial class Route
    {
        public Airport DestinationAirport { get; set; }
        public Airport OriginAirport { get; set; }
        public MarketingCarrier MarketingCarrier { get; set; }
        public List<Segment> Segments { get; set; }
        public bool HasMultiCarrier { get; set; }
        public bool Visible { get; set; }
        public TotalPrice TotalPrice { get; set; }
        public PassengerPrice PassengerPrice { get; set; }
        public string Id { get; set; }
        public string IdStr { get; set; }
        public string RouteFareOptionIdStr { get; set; }
        public string RouteFareOptionId { get; set; }
        public long Duration { get; set; }
        public string ProductCode { get; set; }
        public FreeBaggage FreeBaggage { get; set; }
        public FreeBaggage FreeCabinBaggage { get; set; }
        public string FareClass { get; set; }
        public long CabinClassType { get; set; }
        public long SeatCount { get; set; }
        public bool Alternative { get; set; }
        public object AvailableServices { get; set; }
        public string CabinClassName { get; set; }
    }

    public partial class FreeBaggage
    {
        public long Amount { get; set; }
        public long Unit { get; set; }
        public long? Pieces { get; set; }
    }

    public partial class Segment
    {
        public long Duration { get; set; }
        public string Number { get; set; }
        public List<Leg> Legs { get; set; }
        public string BookingClass { get; set; }
        public object Remark { get; set; }
        public long Stopover { get; set; }
    }

    public partial class Leg
    {
        public DateTimeOffset DepartureTime { get; set; }
        public DateTimeOffset ArrivalTime { get; set; }
        public object Aircraft { get; set; }
        public object OriginTerminal { get; set; }
        public object DestinationTerminal { get; set; }
        public object Duration { get; set; }
        public bool ChangePlane { get; set; }
    }

    public partial class PassengerCounts
    {
        public long Adult { get; set; }
    }

    public enum Currency { Eur };
}

