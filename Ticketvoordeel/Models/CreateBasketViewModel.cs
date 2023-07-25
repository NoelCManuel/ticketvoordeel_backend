using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models.CreateBasket
{
    public class SelectedRoute
    {
        public ulong RouteId { get; set; }
        public ulong RouteFareOptionId { get; set; }

    }

    public class PoolRequest
    {
        public string SearchId { get; set; }
        public string TripId { get; set; }
        public List<SelectedRoute> SelectedRoutes { get; set; }

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

    public class CreateBasketViewModel
    {
        public PoolRequest PoolRequest { get; set; }
        public string CurrencyCode { get; set; }
        public List<Passenger> Passengers { get; set; }

    }
}
