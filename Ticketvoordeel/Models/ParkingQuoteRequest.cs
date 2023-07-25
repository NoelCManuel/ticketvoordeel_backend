using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class ParkingQuoteRequest
    {
        public string couponCode { get; set; }
        public string currencyCode { get; set; }
        public DateTime dateTimeFrom { get; set; }
        public DateTime dateTimeTo { get; set; }
        public string labelAbbreviation { get; set; }
        public string locationAbbreviation { get; set; }
        public string referral { get; set; }
    }

    public class ParkingBookRequest
    {
        public Customer customer { get; set; }
        public string currencyCode { get; set; }
        public object couponCode { get; set; }
        public string enterDateTime { get; set; }
        public string exitDateTime { get; set; }
        public string labelAbbreviation { get; set; }
        public string locationAbbreviation { get; set; }
        public string paymentLocation { get; set; }
        public string posCode { get; set; }
        public string productAbbreviation { get; set; }
        public string referral { get; set; }
        public string externalReference { get; set; }

        public string enableFlightTimeCalculation { get; set; }
    }

    public class Customer
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
        public string flightNumber { get; set; }
        public int numberOfPersons { get; set; }
    }
}
