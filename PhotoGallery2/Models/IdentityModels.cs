using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.DataHandler.Serializer;

namespace PhotoGallery2.Models
{
    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public DbSet<Photo> Photos { get; set; }
    //    public DbSet<Comment> Comments { get; set; }
    //    public DbSet<Album> Albums { get; set; }

    //    public ApplicationDbContext()
    //        : base("PhotoDBContext")
    //    {
            
    //    }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

    //        //    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

    //        //    for (int i = 0; i < 4; i++)
    //        //    {
    //        //        var user = new ApplicationUser()
    //        //        {
    //        //            UserName = string.Format("User {0}", i.ToString())
    //        //        };

    //        //        manager.Create(user, string.Format("Password{0}", i.ToString()));
    //        //    }
    //    }

        
        
    //}
}