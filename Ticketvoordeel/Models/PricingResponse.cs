using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.PricingResponse
{
    public class TotalPrice
    {
        public string Currency { get; set; }
        public double Base { get; set; }
        public double Tax { get; set; }
        public double ServiceFee { get; set; }
        public double AgencyFee { get; set; }
        public double CompanyFee { get; set; }
        public double TotalFeeless { get; set; }
        public double Total { get; set; }
        public object Internal { get; set; }
    }

    public class Name
    {
        public bool IsRequired { get; set; }
        public int LengthMin { get; set; }
        public int LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class Surname
    {
        public bool IsRequired { get; set; }
        public int LengthMin { get; set; }
        public int LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class IdentificationNumber
    {
        public bool IsRequired { get; set; }
        public int LengthMin { get; set; }
        public int LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class IdentificationExpiryDate
    {
        public bool IsRequired { get; set; }
        public object LengthMin { get; set; }
        public object LengthMax { get; set; }
        public DateTime DateMin { get; set; }
        public DateTime DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class IdentificationIssueDate
    {
        public bool IsRequired { get; set; }
        public object LengthMin { get; set; }
        public object LengthMax { get; set; }
        public DateTime DateMin { get; set; }
        public DateTime DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class IdentificationIssueCity
    {
        public bool IsRequired { get; set; }
        public int LengthMin { get; set; }
        public int LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class PossibleValue
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class IdentificationType
    {
        public bool IsRequired { get; set; }
        public object LengthMin { get; set; }
        public object LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public List<PossibleValue> PossibleValues { get; set; }
    }

    public class Mobile
    {
        public bool IsRequired { get; set; }
        public int LengthMin { get; set; }
        public int LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class Phone
    {
        public bool IsRequired { get; set; }
        public int LengthMin { get; set; }
        public int LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class Email
    {
        public bool IsRequired { get; set; }
        public int LengthMin { get; set; }
        public int LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class NationalityCode
    {
        public bool IsRequired { get; set; }
        public int LengthMin { get; set; }
        public int LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class FrequentFlyer
    {
        public bool IsRequired { get; set; }
        public int LengthMin { get; set; }
        public int LengthMax { get; set; }
        public object DateMin { get; set; }
        public object DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class Birthdate
    {
        public bool IsRequired { get; set; }
        public object LengthMin { get; set; }
        public object LengthMax { get; set; }
        public DateTime DateMin { get; set; }
        public DateTime DateMax { get; set; }
        public object PossibleValues { get; set; }
    }

    public class FieldMetadata
    {
        public Name Name { get; set; }
        public Surname Surname { get; set; }
        public IdentificationNumber IdentificationNumber { get; set; }
        public IdentificationExpiryDate IdentificationExpiryDate { get; set; }
        public IdentificationIssueDate IdentificationIssueDate { get; set; }
        public IdentificationIssueCity IdentificationIssueCity { get; set; }
        public IdentificationType IdentificationType { get; set; }
        public Mobile Mobile { get; set; }
        public Phone Phone { get; set; }
        public Email Email { get; set; }
        public NationalityCode NationalityCode { get; set; }
        public FrequentFlyer FrequentFlyer { get; set; }
        public Birthdate Birthdate { get; set; }
    }

    public class TravellerMetadata
    {
        public int Sequence { get; set; }
        public int Type { get; set; }
        public FieldMetadata FieldMetadata { get; set; }
    }

    public class Adult
    {
        public string Currency { get; set; }
        public double Base { get; set; }
        public double Tax { get; set; }
        public double ServiceFee { get; set; }
        public double AgencyFee { get; set; }
        public double CompanyFee { get; set; }
        public double TotalFeeless { get; set; }
        public double Total { get; set; }
        public object Internal { get; set; }
    }

    public class PassengerPrice
    {
        public Adult Adult { get; set; }
        public Adult Child { get; set; }
    }

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
        public object CityName { get; set; }
        public bool Deleted { get; set; }
    }

    public class Destination
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public object CityName { get; set; }
        public bool Deleted { get; set; }
    }

    public class Leg
    {
        public Origin Origin { get; set; }
        public Destination Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public object Aircraft { get; set; }
        public object OriginTerminal { get; set; }
        public object DestinationTerminal { get; set; }
        public object Duration { get; set; }
        public bool ChangePlane { get; set; }
    }

    public class Segment
    {
        public Carrier Carrier { get; set; }
        public object OperatingCarrier { get; set; }
        public int Duration { get; set; }
        public string Number { get; set; }
        public List<Leg> Legs { get; set; }
        public string BookingClass { get; set; }
        public string Remark { get; set; }
        public int Stopover { get; set; }
    }

    public class FreeBaggage
    {
        public double Amount { get; set; }
        public int Unit { get; set; }
        public object Pieces { get; set; }
    }

    public class FreeCabinBaggage
    {
        public double Amount { get; set; }
        public int Unit { get; set; }
        public object Pieces { get; set; }
    }

    public class Route
    {
        public OriginAirport OriginAirport { get; set; }
        public DestinationAirport DestinationAirport { get; set; }
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
        public int Duration { get; set; }
        public string ProductCode { get; set; }
        public string ProviderCode { get; set; }
        public FreeBaggage FreeBaggage { get; set; }
        public FreeCabinBaggage FreeCabinBaggage { get; set; }
        public string FareClass { get; set; }
        public int CabinClassType { get; set; }
        public int SeatCount { get; set; }
        public bool Alternative { get; set; }
        public object AvailableServices { get; set; }
        public string CabinClassName { get; set; }
    }

    public class BookingGroup
    {
        public int Sequence { get; set; }
        public string ProductCode { get; set; }
        public string ProviderCode { get; set; }
        public TotalPrice TotalPrice { get; set; }
        public PassengerPrice PassengerPrice { get; set; }
        public List<Route> Routes { get; set; }
    }

    public class PassengerCounts
    {
        public int Adult { get; set; }
    }

    public class Selection
    {
        public string SearchId { get; set; }
        public int TripType { get; set; }
        public string TripId { get; set; }
        public bool IsMultiTicket { get; set; }
        public bool IsReservable { get; set; }
        public TotalPrice TotalPrice { get; set; }
        public List<TravellerMetadata> TravellerMetadatas { get; set; }
        public List<BookingGroup> BookingGroups { get; set; }
        public PassengerCounts PassengerCounts { get; set; }
        public PassengerPrice PassengerPrice { get; set; }
        public int ResultType { get; set; }
    }

    public class PricingResponse
    {
        public Selection Selection { get; set; }
    }
}
