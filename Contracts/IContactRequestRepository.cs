using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IContactRequestRepository
    {
        IEnumerable<ContactRequest> GetAllContactRequests();
        IEnumerable<ContactRequest> GetContactRequests(Expression<Func<ContactRequest, bool>> predicate);
        ContactRequest GetContactRequestById(int contactRequestId);
        ContactRequest CreateContactRequest(ContactRequest contactRequest);
        bool UpdateContactRequest(ContactRequest contactRequest);
        bool DeleteContactRequest(ContactRequest contactRequest);
    }
}
