using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;

namespace PhotoGallery2.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }
        public string Description { get; set; }
        public int PhotoID { get; set; }

        public virtual string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class UpdateComment
    {
        [Required(ErrorMessage = "Please enter some comment",AllowEmptyStrings = false)]
        public string Description { get; set; }

        public string Comments { get; set; }
        
        public string UserID { get; set; }
        public int PhotoID { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}