using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.BookRequest
{
    public class BookRequest
    {
        public string UserEmail { get; set; }
        public bool IsReturnAvailable { get; set; }
        public Book DepartureBookingObject { get; set; } = new Book();
        public Book ReturnBookingObject { get; set; } = new Book();
        public int AdultsCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
        public List<Passenger> Passengers { get; set; } = new List<Passenger>();
        public bool IsFlightDelayCompensationIncluded { get; set; }
        public double FlightDelayCompensationAmount { get; set; }
        public bool IsInsuranceSelected { get; set; }
        public double DepartureInsuranceAmount { get; set; }
        public double ReturnInsuranceAmount { get; set; }
        public int SelectedServicePackage { get; set; }
        public double ServicePackageAmount { get; set; }
        public bool IsParkingSelected { get; set; }
        public double ParkingAmount { get; set; }
        public string ParkingSelectedAbbreviation { get; set; }
        public ParkingQuoteRequest ParkingQuoteRequest { get; set; } = new ParkingQuoteRequest();
        public int parkingSelectedCount { get; set; }        
        public decimal TotalAmount { get; set; }
        public MainBooker MainBooker { get; set; }
        public SisowPayment sisowPayment { get; set; }
        public int AppliedCredit { get; set; } = 0;
        public List<Discounts> Discounts { get; set; } = new List<Discounts>();
        public List<EternalItems> ExternalItems { get; set; } = new List<EternalItems>();
        public TravelInfo.TravelInfo travelInfo { get; set; }
        public bool IsMerged { get; set; } = false;
    }

    public class ErrorMailRequest
    { 
        public BookRequest BookRequest { get; set; }
        public string TravelDate { get; set; }
        public bool IsErrorinDeparture { get; set; }
        public bool IsErrorInBoth { get; set; }
    }

    public class ContactInformation
    { 
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
    }

    public class Book
    {
        public string BasketKey { get; set; }
        public string BookingLanguageCode { get; set; }
        public string SalesAmount { get; set; }
        public Payment Payment { get; set; } = new Payment();
        public ContactInformation contactInformation { get; set; }
        public ContactInformation CustomerInformation { get; set; }
        public string TravelDate { get; set; }
    }

    public class Passenger
    {
        public string TravellerType { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthdate { get; set; }
        public int CustomerNationalityId { get; set; }
        public int IdentityId { get; set; }
        public string IdentityNumber { get; set; }

    }

    public class Payment
    {
        public string CurrencyCode { get; set; } = "EUR";
        public string PaymentMethodCode { get; set; } = "C";
    }

    public class SisowPayment
    { 
        public string status { get; set; }
        public string trxid { get; set; }
        public string ec { get; set; }
        public string sha1 { get; set; }
    }

    public class MainBooker
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string HouseNumber { get; set; }
        public string PostCode { get; set; }
        public string Place { get; set; }
        public string Street { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class BookResponse
    { 
        public BookRequest Request { get; set; }
        public FinalBookResponse DepartureBookResponse { get; set; }
        public FinalBookResponse ReturnBookResponse { get; set; }
        public TravelInfo.TravelInfo travelInfo { get; set; }
        public List<string> ParkingPNRList { get; set; }
        public bool BookingSucceeded { get; set; }
        public string DepartureInsurancePolicyNumber { get; set; }
        public string ReturnInsurancePolicyNumber { get; set; }
        public string ParkingNumber { get; set; }
        public bool IsParkingSucceeded { get; set; } = false;
        public string TransactionId { get; set; }
        public string UserEmail { get; set; }
        public string SuvendusApplied { get; set; }
    }

    public class Discounts
    { 
        public string BasketKey { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
    }

    public class EternalItems
    {
        public string BasketKey { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
    }

    public class FinalBookRequest
    { 
        public string TransactionId { get; set; }
        public SisowPayment sisowPayment { get; set; }
    }
}
