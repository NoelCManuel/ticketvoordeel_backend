using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.ParkingResponse
{
    public class Currency
    {
        public string currencyCode { get; set; }
        public string currencyName { get; set; }
        public double currencyRate { get; set; }
        public string currencySymbol { get; set; }
    }

    public class Vehicle
    {
        public object brand { get; set; }
        public object model { get; set; }
        public object color { get; set; }
        public object registrationPlate { get; set; }
    }

    public class Customer
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
        public string flightNumber { get; set; }
        public int numberOfPersons { get; set; }
        public Vehicle vehicle { get; set; }
    }

    public class BusinessAccount
    {
        public string name { get; set; }
    }

    public class Location
    {
        public string locationName { get; set; }
        public string parkingName { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string pickupInstructions { get; set; }
        public string routeDescription { get; set; }
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
        public CouponDiscount couponDiscount { get; set; }
        public ReferralAdjustment referralAdjustment { get; set; }
    }

    public class ParkingResponse
    {
        public string code { get; set; }
        public DateTime created { get; set; }
        public bool isValid { get; set; }
        public string language { get; set; }
        public DateTime enterDateTime { get; set; }
        public DateTime exitDateTime { get; set; }
        public int numberOfDays { get; set; }
        public string productName { get; set; }
        public bool isValet { get; set; }
        public string referralName { get; set; }
        public string labelName { get; set; }
        public string status { get; set; }
        public string externalReference { get; set; }
        public Currency currency { get; set; }
        public Customer customer { get; set; }
        public BusinessAccount businessAccount { get; set; }
        public Location location { get; set; }
        public Pricing pricing { get; set; }
        public List<object> services { get; set; }
    }
}
