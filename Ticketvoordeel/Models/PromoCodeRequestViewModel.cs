using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class PromoCodeRequestViewModel
    {
        public string CouponCode { get; set; }
        public string BasketKey { get; set; }
        public string Email { get; set; }
    }

    public class PromotionResponse
    {
        public bool IsOk { get; set; }
        public string Message { get; set; }
    }
}
