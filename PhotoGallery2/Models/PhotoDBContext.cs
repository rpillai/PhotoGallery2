using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace PhotoGallery2.Models
{
    public class PhotoDBContext 
        : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Comment> Comments { get; set; }
        
        public PhotoDBContext()
            : base("PhotoDBContext")
        {
            
        }

        public static PhotoDBContext Create()
        {
            return new PhotoDBContext();
        }
    }
}