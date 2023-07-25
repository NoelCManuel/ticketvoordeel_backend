using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public string Mobile { get; set; }
        public string ResetCode { get; set; }
        public Profile()
        {
            CreationTime = DateTime.Now;
            IsActive = true;
            IsVerified = true;
        }
    }
}
