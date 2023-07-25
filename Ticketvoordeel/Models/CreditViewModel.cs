using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class CreditViewModel
    {
        public DateTime CreatedDate { get; set; }
        public string Credits { get; set; }
        public string Description { get; set; }
        public string AvailableCredits { get; set; }
    }

    public class CreditCouponViewModel
    {
        public int TotalCouponAvailable { get; set; }
        public int TotalCouponsApplicable { get; set; }
        public decimal AmountToDeduct { get; set; }
    }

    public class CreditListing
    { 
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CreditsAvailable { get; set; }
        public string CreditsApplied { get; set; }
        public string CreditsReceived { get; set; }
    }

}
