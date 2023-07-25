using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public Test()
        {
            CreationTime = DateTime.Now;
        }
    }
}
