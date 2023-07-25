using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IBookingRepository
    {
        IEnumerable<Booking> GetAllBooking();
        IEnumerable<Booking> GetBooking(Expression<Func<Booking, bool>> predicate);
        Booking GetBookingById(int bookingId);
        Booking CreateBooking(Booking booking);
        bool UpdateBooking(Booking booking);
        bool DeleteBooking(Booking booking);
    }
}
