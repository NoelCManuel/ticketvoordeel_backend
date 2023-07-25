using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class FaqRequest
    {
        public string Message { get; set; }
        public string Email { get; set; }
    }
    public class IncompleteBookingDetails
    {
        public string imageData { get; set; }
        public string personalDetails { get; set; }
        public string step { get; set; }
        public string uniqueId { get; set; }
        public string FirstName { get; set; }
        public string FirstEmail { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ContactDetails
    {
        public int country { get; set; }
        public int code { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthdate { get; set; }
        public Dob Dob { get; set; }
        public string streetName { get; set; }
        public string postelCode { get; set; }
        public string email { get; set; }
        public string confirmEmail { get; set; }
        public int mobile { get; set; }
        public string houseNumber { get; set; }
        public string Place { get; set; }
    }

    public class Date
    {
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
    }

    public class Dob
    {
        public Date date { get; set; }
        public DateTime jsdate { get; set; }
        public string formatted { get; set; }
        public int epoc { get; set; }
    }

    public class FinalPassengersList
    {
        public string TravellerType { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string Birthdate { get; set; }
        public Dob Dob { get; set; }
        public string IdentityNumber { get; set; }
        public int CustomerNationalityId { get; set; }
        public int IdentityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class IncompleteBookingParams
    {
        public List<FinalPassengersList> finalPassengersList { get; set; }
        public ContactDetails contactDetails { get; set; }
    }


}
