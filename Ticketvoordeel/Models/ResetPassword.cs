using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Models
{
    public class ResetPassword
    {
        public string Email { get; set; }
    }

    public class UpdatePassword
    {
        public string Code { get; set; }
        public string Password { get; set; }
    }
}
