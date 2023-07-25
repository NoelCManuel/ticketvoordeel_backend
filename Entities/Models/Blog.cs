using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string LinkName { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
        public Blog()
        {
            CreationTime = DateTime.Now;
        }
    }
}
