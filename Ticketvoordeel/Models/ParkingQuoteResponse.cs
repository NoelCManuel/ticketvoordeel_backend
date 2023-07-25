using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.ParkingQuoteResponse
{
    public class Currency
    {
        public string currencyCode { get; set; }
        public string currencyName { get; set; }
        public double currencyRate { get; set; }
        public string currencySymbol { get; set; }
    }

    public class CouponDiscount
    {
        public object couponCode { get; set; }
        public bool couponValid { get; set; }
        public object couponDescription { get; set; }
        public double amount { get; set; }
    }

    public class ReferralAdjustment
    {
        public double amount { get; set; }
    }

    public class Pricing
    {
        public double pricePerDay { get; set; }
        public double regularPrice { get; set; }
        public double vatAmount { get; set; }
        public double vatRate { get; set; }
        public double totalPrice { get; set; }
        public double refundablePercentage { get; set; }
        public string selectedPaymentLocation { get; set; }
        public List<string> supportedPaymentLocations { get; set; }
        public List<object> supportedPaymentMethods { get; set; }
        public List<object> supportedCreditCards { get; set; }
        public CouponDiscount couponDiscount { get; set; }
        public ReferralAdjustment referralAdjustment { get; set; }
    }

    public class DisplayName
    {
        public string language { get; set; }
        public string text { get; set; }
    }

    public class GeoCoordinate
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
    }

    public class Product
    {
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string description { get; set; }
        public bool isSoldOut { get; set; }
        public bool isValet { get; set; }
        public int availableSpots { get; set; }
        public Pricing pricing { get; set; }
        public List<object> services { get; set; }
        public List<DisplayName> displayNames { get; set; }
        public List<GeoCoordinate> geoCoordinates { get; set; }
    }

    public class ParkingQuoteResponse
    {
        public Currency currency { get; set; }
        public List<Product> products { get; set; }
    }
}
