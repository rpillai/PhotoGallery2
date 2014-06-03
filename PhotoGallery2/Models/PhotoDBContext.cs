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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new PhotoDBContext()));

            for (int i = 0; i < 4; i++)
            {
                var user = new ApplicationUser()
                {
                    UserName = string.Format("User {0}", i.ToString())
                };

                manager.Create(user, string.Format("Password{0}", i.ToString()));
            }
        }

        //public System.Data.Entity.DbSet<PhotoGallery2.Models.ApplicationUser> IdentityUsers { get; set; }
    }
}