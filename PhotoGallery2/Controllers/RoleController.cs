using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.ClientServices.Providers;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Serialization;
using PhotoGallery2.Models;

namespace PhotoGallery2.Controllers
{
    [Authorize(Roles="Administrator") ]
    public class RoleController : Controller
    {
        private PhotoDBContext context;
        public ApplicationRoleManager _roleManager { get; set; }
        public ApplicationUserManager _userManager { get; set; } 

        public RoleController()
        {
            context = new PhotoDBContext();
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        public ApplicationRoleManager RoleManager
        {
            get { return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>(); }

            set { _roleManager = value; }
        }



        public ActionResult Index()
        {
            var roles = context.Roles;
            
            return View(roles.ToList());
        }

        public ActionResult Create()
        {
            return View(new RoleViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = RoleManager.RoleExistsAsync(model.Name);

                if (result.Result == false)
                {
                    var roleCreate = await RoleManager.CreateAsync(new IdentityRole(model.Name));

                    if (roleCreate.Succeeded)
                    {
                        return RedirectToAction("Index", "Role");
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "An error occured.");
                    }
                }
            }

            return View(model);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var role = await RoleManager.FindByIdAsync(id);
                context.Roles.Remove(role);
                context.SaveChanges();

                return RedirectToAction("Index", "Role");
            }
            return View();
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(RoleManager.FindById(id));
        }

        public ActionResult UserRoles(string userId)
        {
           //context = new PhotoDBContext();
            if(string.IsNullOrEmpty(userId) == false)
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == userId);

                if(user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var model = new SelectUserRolesViewModel(user);
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = context.Users.First(u => u.UserName == model.UserName);

                foreach (var role in context.Roles)
                {
                    UserManager.RemoveFromRoleAsync(user.Id, role.Name);
                }

                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        UserManager.AddToRole(user.Id, role.RoleName);
                    }
                }

                return RedirectToAction("Index");
            }

            return View();
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
                RoleManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
