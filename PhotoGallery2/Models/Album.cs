using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoGallery2.Models
{
    public class Album
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int AlbumID { get; set; }

        [DisplayName("Album Name")]
        [Required(ErrorMessage = "Album name cannot be empty",AllowEmptyStrings = false)]
        [StringLength(50)]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Created")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Created date cannot be empty")]
        public DateTime CreateDate { get; set; }

        [DisplayName("Place")]
        public string PlaceTaken { get; set; }

        [DisplayName("Key Photo Path")]
        public string KeyPhotoPath { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
    }
}