using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class ContactRequest
    {
        public int Id { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BookingNumber { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public ContactRequest()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
