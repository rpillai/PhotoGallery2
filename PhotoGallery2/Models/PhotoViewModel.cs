using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoGallery2.Models
{
    public class PhotoViewModel
    {
        public int PhotoID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }
        public string PhotoPath { get; set; }
    }
}