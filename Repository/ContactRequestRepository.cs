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
    public class ContactRequestRepository : RepositoryBase<ContactRequest>, IContactRequestRepository
    {
        public ContactRequestRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<ContactRequest> GetAllContactRequests()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<ContactRequest> GetContactRequests(Expression<Func<ContactRequest, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public ContactRequest GetContactRequestById(int id)
        {
            return FindByCondition(c => c.Id.Equals(id))
                .FirstOrDefault();
        }

        public ContactRequest CreateContactRequest(ContactRequest contactRequest)
        {
            try
            {
                Create(contactRequest);
                return contactRequest;
            }
            catch (Exception ex)
            {
                return contactRequest;
            }
        }

        public bool UpdateContactRequest(ContactRequest contactRequest)
        {
            try
            {
                Update(contactRequest);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteContactRequest(ContactRequest contactRequest)
        {
            try
            {
                Delete(contactRequest);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
