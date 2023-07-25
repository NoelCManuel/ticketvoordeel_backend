using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.PriceRequests
{
    public class SelectedRoute
    {
        public string RouteId { get; set; }
        public string RouteFareOptionId { get; set; }
    }

    public class PoolRequest
    {
        public string SearchId { get; set; }
        public string TripId { get; set; }
        public List<SelectedRoute> SelectedRoutes { get; set; }
    }

    public class PriceRequest
    {
        public PoolRequest PoolRequest { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class PriceResponses
    {
        public bool IsCombined { get; set; } = false;
        public PricingResponse.PricingResponse PricingResponse { get; set; }
    }
}
