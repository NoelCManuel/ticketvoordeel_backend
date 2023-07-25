using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.TwoWayResponse
{
    public class PassengerCounts
    {
        public int Adult { get; set; }
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

    public class FareGroup
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
        public bool InPage { get; set; }
        public bool IsMultiTicket { get; set; }
        public bool HasMultiCarrier { get; set; }
        //public List<List<Routes>> Routes { get; set; }
        public TotalPrice TotalPrice { get; set; }
        public PassengerPrice PassengerPrice { get; set; }
    }

    public class Reader
    {
        public string Currency { get; set; }
        public string SearchId { get; set; }
        public int TripType { get; set; }
        public int ResultType { get; set; }
        public bool ExcludeServiceFee { get; set; }
        public bool UseTotalPrice { get; set; }
        public PassengerCounts PassengerCounts { get; set; }
        public int RouteDuration { get; set; }
        public List<FareGroup> FareGroups { get; set; }
        public bool ContainsAlternatives { get; set; }
        public int TotalGroupCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
    }

    public class TwoWayResponse
    {
        public Reader Reader { get; set; }
    }
}
