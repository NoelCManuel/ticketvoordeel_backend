using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IIncompleteBookingRepository
    {
        IEnumerable<IncompleteBooking> GetAllIncompleteBooking();
        IncompleteBooking GetIncompleteBookingById(int incompleteBookingId);
        IncompleteBooking CreateIncompleteBooking(IncompleteBooking incompleteBooking);
        bool UpdateIncompleteBooking(IncompleteBooking incompleteBooking);
        bool DeleteIncompleteBooking(IncompleteBooking incompleteBooking);
    }
}
