using System;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using PhotoGallery2.Models;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PhotoGallery2.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<PhotoDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "PhotoDBContext";
        }

        protected override void Seed(PhotoDBContext context)
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

            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

            const string roleName = "Administrator";

            var role = roleManager.FindByName(roleName);

            if (role == null)
            {
                role = new IdentityRole(roleName);
                roleManager.Create(role);
            }

            var newUser = userManager.FindByName("ramesh.pillai@gmail.com");

            if (newUser == null)
            {
                newUser = new ApplicationUser
                {
                    FirstName = "Ramesh",
                    LastName = "Pillai",
                    UserName = "ramesh.pillai@gmail.com",
                    Email = "ramesh.pillai@gmail.com",
                    EmailConfirmed = true
                };

                var success = userManager.Create(newUser, "Password1");
                success = userManager.SetLockoutEnabled(newUser.Id, false);
            }

            var rolesForUser = userManager.GetRoles(newUser.Id);

            if (rolesForUser.Contains(role.Name) == false)
            {
                userManager.AddToRole(newUser.Id, roleName);
            }

            base.Seed(context);

        }
    }

}
