using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreationTime { get; set; }
        public string IsActive { get; set; }
        public string InActiveNote { get; set; }

    }
}
