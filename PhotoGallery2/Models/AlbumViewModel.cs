using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoGallery2.Models
{
    public class AlbumViewModel
    {
        public int AlbumID { get; set; }
        public string AlbumName { get; set; }
        public string Description { get; set; }
        public DateTime? DateTaken { get; set; }
        public string KeythumbnailPath { get; set; }
        public int PhotoCount { get; set; }
    }
}