using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string LinkTo { get; set; }
        public DateTime CreationTime { get; set; }
        public Banner()
        {
            CreationTime = DateTime.Now;
        }
    }
}
