using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.PriceRequest
{
    public class SelectedRoute
    {
        public object RouteId { get; set; }
        public object RouteFareOptionId { get; set; }

    }

    public class PoolRequest
    {
        public string SearchId { get; set; }
        public string TripId { get; set; }
        public List<SelectedRoute> SelectedRoutes { get; set; }

    }

    public class AirpoolPricingRequestViewModel
    {
        public PoolRequest PoolRequest { get; set; }
        public string CurrencyCode { get; set; }

    }
}
