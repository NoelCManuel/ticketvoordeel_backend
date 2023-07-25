using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.Booking
{
    public class ExternalItemRequest
    {
        public string BasketKey { get; set; }
        public string CurrencyCode { get; set; }
        public Item[] Items { get; set; }
    }

    public partial class Item
    {
        public string Explain { get; set; }
        public long OrderNumber { get; set; }
        public long ItemCount { get; set; }
        public string Temp { get; set; }
        public Price[] Prices { get; set; }
    }

    public partial class Price
    {
        public double Amount { get; set; }
        public string PriceExplain { get; set; }
    }
}
