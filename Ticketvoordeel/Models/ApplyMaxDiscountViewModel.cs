using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class ApplyMaxDiscountViewModel
    {
        public string UserId { get; set; }
        public string BasketKey { get; set; }
    }

    public class MaxDiscountReturnViewModel
    {
        public string DiscountName { get; set; }
        public int DiscountAmount { get; set; }
        public bool IsApplicable { get; set; } = false;
    }
}
