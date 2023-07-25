using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class DynamicPage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FromAirport { get; set; }
        public string ToAirport { get; set; }
        public string FromAirportCode { get; set; }
        public string ToAirportCode { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string ShortDescription { get; set; }
        public DynamicPage()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
