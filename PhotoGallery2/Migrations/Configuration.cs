using PhotoGallery2.Models;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PhotoGallery2.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PhotoGallery2.Models.PhotoDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "PhotoGallery2.Models.PhotoDBContext";
        }

        protected override void Seed(PhotoGallery2.Models.PhotoDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            var albums = new List<Album>
            {
                new Album
                {
                    Name = "Animals",
                    Description = "List of Animals",
                    CreateDate = DateTime.Now
                },
                new Album
                {
                    Name = "Flowers",
                    Description = "List of Flowers",
                    CreateDate = DateTime.Now
                },
                new Album
                {
                    Name = "Places",
                    Description = "List of Places",
                    CreateDate = DateTime.Now
                }
            };

            albums.ForEach(a => context.Albums.AddOrUpdate(n => n.Name, a));
            context.SaveChanges();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            IdentityResult role = null;

            if (roleManager.RoleExists("Administrator") == false)
                role = roleManager.Create(new IdentityRole("Administrator"));

            if (context.Users.FirstOrDefault(u => u.UserName == "rpillai") == null)
            {
                var newUser = new ApplicationUser
                {
                    FirstName = "Ramesh",
                    LastName = "Pillai",
                    UserName = "rpillai",
                    Id = Guid.NewGuid().ToString()
                };

                var success = userManager.Create(newUser, "Password1");

                if (role != null
                    && role.Succeeded)
                    userManager.AddToRole(newUser.Id, "Administrator");
            }

            if (roleManager.RoleExists("testRole") == false)
                role = roleManager.Create(new IdentityRole("testRole"));


        }
    }

}
