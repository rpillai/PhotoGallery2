using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PhotoGallery2.Models;

namespace PhotoGallery2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserAdminController : Controller
    {
        public UserAdminController()
        {
            
        }

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public UserAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }


        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        public ApplicationRoleManager RoleManager
        {
            get { return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>() ; }
            set { _roleManager = value; }
        }

        // GET: UserAdmin
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }


        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound();

            var userRoles = await UserManager.GetRolesAsync(id);

            return View(new UserEditViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select( x=> new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Name,
                    Selected = userRoles.Contains(x.Name)
                })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include =  "Email,Id")] UserEditViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] {};

                foreach (var userRole in userRoles)
                {
                    var result = await UserManager.RemoveFromRoleAsync(user.Id, userRole);
                }
                
                foreach (var roleName in selectedRole)
                {
                    var result = await UserManager.AddToRoleAsync(user.Id, roleName);    
                }

                return RedirectToAction("Index");
            }

            return View();
        }


        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(user);
        }
        
    }
}