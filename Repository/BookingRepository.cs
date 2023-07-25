using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository
{
    public class BookingRepository : RepositoryBase<Booking>, IBookingRepository
    {
        public BookingRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Booking> GetAllBooking()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<Booking> GetBooking(Expression<Func<Booking, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public Booking GetBookingById(int testId)
        {
            return FindByCondition(test => test.Id.Equals(testId))
                .FirstOrDefault();
        }

        public Booking CreateBooking(Booking booking)
        {
            try
            {
                Create(booking);
                return booking;
            }
            catch (Exception ex)
            {
                return booking;
            }
        }

        public bool UpdateBooking(Booking booking)
        {
            try
            {
                Update(booking);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteBooking(Booking booking)
        {
            try
            {
                Delete(booking);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
