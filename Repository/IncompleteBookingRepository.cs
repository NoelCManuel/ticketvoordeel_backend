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
    public class IncompleteBookingRepository : RepositoryBase<IncompleteBooking>, IIncompleteBookingRepository
    {

        public IncompleteBookingRepository(RepositoryContext repositoryContext)
    : base(repositoryContext)
        {
        }

        public IEnumerable<IncompleteBooking> GetAllIncompleteBooking()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IncompleteBooking GetIncompleteBookingById(int testId)
        {
            return FindByCondition(test => test.Id.Equals(testId))
                .FirstOrDefault();
        }

        public IncompleteBooking CreateIncompleteBooking(IncompleteBooking IncompleteBooking)
        {
            try
            {
                Create(IncompleteBooking);
                return IncompleteBooking;
            }
            catch (Exception ex)
            {
                return IncompleteBooking;
            }
        }

        public bool UpdateIncompleteBooking(IncompleteBooking IncompleteBooking)
        {
            try
            {
                Update(IncompleteBooking);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteIncompleteBooking(IncompleteBooking IncompleteBooking)
        {
            try
            {
                Delete(IncompleteBooking);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
