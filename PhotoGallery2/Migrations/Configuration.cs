using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoGallery2.Models;

namespace PhotoGallery2.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<PhotoDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PhotoDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            var albums = new List<Album>
            {
                new Album
                {
                    Name = "Test Album1",
                    Description = "Description for the Test Album One",
                    CreateDate = DateTime.Now
                },
                new Album
                {
                    Name = "Test Album2",
                    Description = "Description for the Test Album Two",
                    CreateDate = DateTime.Now
                },
                new Album
                {
                    Name = "Test Album3",
                    Description = "Description for the Test Album Three",
                    CreateDate = DateTime.Now
                },
                new Album
                {
                    Name = "Test Album4",
                    Description = "Description for the Test Album Four",
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
                    Id =  Guid.NewGuid().ToString()
                };

                var success = userManager.Create(newUser, "Password1");

                if(role !=null 
                    && role.Succeeded)
                    userManager.AddToRole(newUser.Id, "Administrator");
            }

            if (roleManager.RoleExists("testRole") == false)
                role = roleManager.Create(new IdentityRole("testRole"));

            
        }
    }
}
