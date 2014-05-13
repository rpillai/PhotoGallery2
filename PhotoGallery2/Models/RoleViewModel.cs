using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PhotoGallery2.Models
{
    public class RoleViewModel 
    {

        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Role Name is Required.")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}