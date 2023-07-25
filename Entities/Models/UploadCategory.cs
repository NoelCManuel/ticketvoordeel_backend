using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class UploadCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime CreationTime { get; set; }
        public UploadCategory()
        {
            CreationTime = DateTime.Now;
        }
    }
}
