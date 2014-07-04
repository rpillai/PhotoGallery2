using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoGallery2.Models
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PhotoID { get; set; }

        public string Title { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        
        public string Place { get; set; }
        public string PhotoPath { get; set; }
        public string ThumbnailPath { get; set; }

        public string ContentType { get; set; }
        
        public int AlbumID { get; set; }

        public virtual Album Album { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } 
    }

    public class ManagePhotoModel
    {
        public int PhotoID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AlbumName { get; set; }
        public string ThumpnailPath { get; set; }
    }
}