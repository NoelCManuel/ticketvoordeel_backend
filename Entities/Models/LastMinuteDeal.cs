using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    public class LastMinuteDeal
    {
        public int Id { get; set; }
        public string PoolSearchRequest { get; set; }
        public bool IsActive { get; set; }
        public int ColumnNumber { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string Airline { get; set; }
        public LastMinuteDeal()
        {
            CreationTime = DateTime.Now;
        }
    }
}
