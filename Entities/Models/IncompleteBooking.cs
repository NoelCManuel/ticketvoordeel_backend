using System;
using System.Collections.Generic;
using System.Text;
namespace Entities.Models
{
    public class IncompleteBooking
    {
        public int Id { get; set; }
        public string UniqueId { get; set; }
        public string personalDetails { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string steponepath { get; set; }
        public string steptwopath { get; set; }
        public string stepthreepath { get; set; }
        public string stepfourpath { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsMailSent { get; set; }
    }
}
