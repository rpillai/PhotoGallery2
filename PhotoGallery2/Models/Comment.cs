using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;

namespace PhotoGallery2.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        public string Comments { get; set; }
        public int PhotoID { get; set; }
    }
}