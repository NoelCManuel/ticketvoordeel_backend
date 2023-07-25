using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class ImageRepository
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string FileName { get; set; }
        public string OriginalName { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public DateTime CreationTime { get; set; }
        public ImageRepository()
        {
            CreationTime = DateTime.Now;
        }
    }
}
