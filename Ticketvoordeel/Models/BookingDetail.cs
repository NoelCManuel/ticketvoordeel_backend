//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Ticketvoordeel.Models
//{
//    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

//    public class RelativeInfo
//    {
//        public int ID { get; set; }
//        public int BookingID { get; set; }
//        public object Name { get; set; }
//        public object Surname { get; set; }
//        public DateTime BirthDate { get; set; }
//        public int TitleID { get; set; }
//        public int LanguageID { get; set; }
//        public object PostalCode { get; set; }
//        public object Address { get; set; }
//        public int CountryID { get; set; }
//        public object City { get; set; }
//        public object Email { get; set; }
//        public object PhoneNumber1 { get; set; }
//        public object PhoneNumber2 { get; set; }
//        public object Remark { get; set; }
//        public object Attention { get; set; }
//        public int CustomerNationalityID { get; set; }
//        public int IdentityID { get; set; }
//        public object IdentityNumber { get; set; }
//    }

//    public class BookingTraveller
//    {
//        public int Id { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public DateTime BirthDate { get; set; }
//        public string Title { get; set; }
//        public int TravellerType { get; set; }
//        public int CustomerNationalityId { get; set; }
//        public object NationalityCode { get; set; }
//        public int IdentityId { get; set; }
//        public string IdentityNumber { get; set; }
//        public object TravellerTitle { get; set; }
//        public object TravellerInformation { get; set; }
//        public object FieldMetadata { get; set; }
//        public string LoyaltyReferenceNumber { get; set; }
//        public object FrequentFlyerNumber { get; set; }
//        public object HealthCheckCode { get; set; }
//    }

//    public class FareClass
//    {
//        public string Code { get; set; }
//        public string Name { get; set; }
//    }

    

//    public class Arrival
//    {
//        public string Code { get; set; }
//        public string Name { get; set; }
//    }

//    public class Airline
//    {
//        public string Code { get; set; }
//        public string Name { get; set; }
//    }

//    public class ProductTraveller
//    {
//        public string TicketNumber { get; set; }
//        public List<int> BookingProductRelationIds { get; set; }
//        public object IsOpen { get; set; }
//        public int Id { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public DateTime BirthDate { get; set; }
//        public string Title { get; set; }
//        public int TravellerType { get; set; }
//        public object CustomerNationalityId { get; set; }
//        public object NationalityCode { get; set; }
//        public object IdentityId { get; set; }
//        public string IdentityNumber { get; set; }
//        public object TravellerTitle { get; set; }
//        public object TravellerInformation { get; set; }
//        public object FieldMetadata { get; set; }
//        public string LoyaltyReferenceNumber { get; set; }
//        public object FrequentFlyerNumber { get; set; }
//        public object HealthCheckCode { get; set; }
//    }

//    public class Leg
//    {
//        public int BoundType { get; set; }
//        public int SequenceNumber { get; set; }
//        public string FlightNumber { get; set; }
//        public DateTime DepartureDate { get; set; }
//        public string DepartureAirportCode { get; set; }
//        public string DepartureAirportName { get; set; }
//        public DateTime ArriveDate { get; set; }
//        public string ArriveAirportCode { get; set; }
//        public string ArriveAirportName { get; set; }
//        public string AirlineCode { get; set; }
//        public string AirlineName { get; set; }
//        public object MarketingAirlineCode { get; set; }
//        public object MarketingAirlineName { get; set; }
//        public string FlightClassCode { get; set; }
//        public string FlightClassName { get; set; }
//        public object CabinClass { get; set; }
//        public int BaggageType { get; set; }
//        public int BaggageKg { get; set; }
//        public int HandLuggageType { get; set; }
//        public int HandLuggageKg { get; set; }
//        public string FareName { get; set; }
//    }

//    public class BookingProductDetail
//    {
//        public FareClass FareClass { get; set; }
//        public Departure Departure { get; set; }
//        public Arrival Arrival { get; set; }
//        public Airline Airline { get; set; }
//        public string FlightProductSource { get; set; }
//        public string FlightNumber { get; set; }
//        public string ProviderNumber { get; set; }
//        public string OperatorNumber { get; set; }
//        public string ProviderName { get; set; }
//        public object OptionDate { get; set; }
//        public List<ProductTraveller> ProductTravellers { get; set; }
//        public List<object> SpecialServices { get; set; }
//        public List<Leg> Legs { get; set; }
//        public int AdultCount { get; set; }
//        public int ChildCount { get; set; }
//        public int InfantCount { get; set; }
//        public DateTime BeginDate { get; set; }
//        public DateTime EndDate { get; set; }
//        public List<object> Prices { get; set; }
//        public int SubTotal { get; set; }
//    }

//    public class BookingProduct
//    {
//        public int BookingLineId { get; set; }
//        public int BookingProductId { get; set; }
//        public int ReservationType { get; set; }
//        public List<BookingProductDetail> BookingProductDetails { get; set; }
//        public int CommissionAmount { get; set; }
//    }

//    public class Installment
//    {
//        public int InstallmentId { get; set; }
//        public string InstallmentName { get; set; }
//        public int InstallmentCount { get; set; }
//        public double BaseAmount { get; set; }
//        public double PosAmount { get; set; }
//        public double Supplement { get; set; }
//        public double Total { get; set; }
//        public string CurrencyCode { get; set; }
//    }

//    public class PaymentMethodList
//    {
//        public int PosId { get; set; }
//        public string LogoClass { get; set; }
//        public bool PreCardCheck { get; set; }
//        public bool HasParameters { get; set; }
//        public string PosCode { get; set; }
//        public string PosName { get; set; }
//        public int PosType { get; set; }
//        public bool ForLoyalty { get; set; }
//        public string PosInformation { get; set; }
//        public List<Installment> Installments { get; set; }
//    }

//    public class CustomerPaymentMethodList
//    {
//        public int PosId { get; set; }
//        public string LogoClass { get; set; }
//        public bool PreCardCheck { get; set; }
//        public bool HasParameters { get; set; }
//        public string PosCode { get; set; }
//        public string PosName { get; set; }
//        public int PosType { get; set; }
//        public bool ForLoyalty { get; set; }
//        public string PosInformation { get; set; }
//        public List<Installment> Installments { get; set; }
//    }

//    public class FinanceDetail2
//    {
//        public bool CanCancel { get; set; }
//        public bool CanModify { get; set; }
//        public List<PaymentMethodList> PaymentMethodList { get; set; }
//        public List<CustomerPaymentMethodList> CustomerPaymentMethodList { get; set; }
//        public List<object> DepositPaymentMethodList { get; set; }
//        public object QuotaStatusList { get; set; }
//    }

//    public class Currency
//    {
//        public string Symbol { get; set; }
//        public string ISONumericCode { get; set; }
//        public string ShowText { get; set; }
//        public bool EnableForSales { get; set; }
//        public int Id { get; set; }
//        public string Code { get; set; }
//        public string Name { get; set; }
//    }

//    public class AgentInfo
//    {
//        public int Id { get; set; }
//        public int OwnerAgentID { get; set; }
//        public int AgentType { get; set; }
//        public object Code { get; set; }
//        public string Name { get; set; }
//        public object OwnerTitle { get; set; }
//        public int OwnerGender { get; set; }
//        public object OwnerName { get; set; }
//        public object OwnerSurname { get; set; }
//        public object Email { get; set; }
//        public object Phone { get; set; }
//        public object Mobile { get; set; }
//        public string LanguageCode { get; set; }
//        public object Address { get; set; }
//        public object City { get; set; }
//        public object PostCode { get; set; }
//        public object Country { get; set; }
//        public bool PrePaymentFlag { get; set; }
//    }

//    public class CompanyInfo
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public object CurrencyCode { get; set; }
//        public string LanguageCode { get; set; }
//        public object Country { get; set; }
//        public object CustomerCountry { get; set; }
//        public bool NeedRelationKey { get; set; }
//        public object TursysApiClientId { get; set; }
//        public object TursysApiClientSecret { get; set; }
//        public object TursysApiUrl { get; set; }
//    }

//    public class BookingCollectingPayment
//    {
//        public string Pnr { get; set; }
//        public double Amount { get; set; }
//        public double MainAmount { get; set; }
//        public int MainAmountParity { get; set; }
//        public double SourceAmount { get; set; }
//        public string PaymentType { get; set; }
//        public string PaymentRelation { get; set; }
//        public int SourceAmountParity { get; set; }
//        public DateTime TransactionDate { get; set; }
//        public string CurrencySymbol { get; set; }
//        public string MainAmountCurrencySymbol { get; set; }
//        public string SourceAmountCurrencySymbol { get; set; }
//        public string TransactionType { get; set; }
//        public string OrderNumber { get; set; }
//        public int InstallmentCount { get; set; }
//        public string PosName { get; set; }
//        public string CreditCardNumber { get; set; }
//        public string RelationNumber { get; set; }
//    }

//    public class Accounting
//    {
//        public List<object> BookingInvoiceList { get; set; }
//        public List<BookingCollectingPayment> BookingCollectingPayments { get; set; }
//    }

//    public class CustomerCountry
//    {
//        public string Code2 { get; set; }
//        public int OrderNo { get; set; }
//        public string PhoneCode { get; set; }
//        public int Id { get; set; }
//        public string Code { get; set; }
//        public string Name { get; set; }
//    }

//    public class InvoiceContact
//    {
//        public int ContactType { get; set; }
//        public string ContactName { get; set; }
//        public string CorporateName { get; set; }
//        public string TaxOffice { get; set; }
//        public string TaxNumber { get; set; }
//        public string Address { get; set; }
//        public string CityName { get; set; }
//        public string PostCode { get; set; }
//        public string Email { get; set; }
//        public CustomerCountry CustomerCountry { get; set; }
//    }

//    public class BookingDetail
//    {
//        public int BookingId { get; set; }
//        public string PinCode { get; set; }
//        public object PromotionPrice { get; set; }
//        public object PromotionExplain { get; set; }
//        public string Pnr { get; set; }
//        public object Creator { get; set; }
//        public Customer Customer { get; set; }
//        public RelativeInfo RelativeInfo { get; set; }
//        public List<BookingTraveller> BookingTravellers { get; set; }
//        public List<BookingProduct> BookingProducts { get; set; }
//        public Currency Currency { get; set; }
//        public AgentInfo AgentInfo { get; set; }
//        public CompanyInfo CompanyInfo { get; set; }
//        public List<object> BookingCoupons { get; set; }
//        public DateTime TravelDate { get; set; }
//        public int BookingQuotaStatus { get; set; }
//        public string BookingStatus { get; set; }
//        public DateTime BookingDate { get; set; }
//        public DateTime ModificationDate { get; set; }
//        public double TotalSalesAmount { get; set; }
//        public int TotalCommissionAmount { get; set; }
//        public int TotalServiceChargeAmount { get; set; }
//        public object DepositAmount { get; set; }
//        public bool IsContainsExternalProduct { get; set; }
//        public string RelationKey { get; set; }
//        public string Remark { get; set; }
//        public object OptionDate { get; set; }
//        public object DepositDate { get; set; }
//        public bool ConfirmationSendStatus { get; set; }
//        public string SalesSource { get; set; }
//    }

//    public class BookingDetailRequest
//    { 
//        public string Pnr { get; set; }
//        public int Section { get; set; }
//    }
//}
