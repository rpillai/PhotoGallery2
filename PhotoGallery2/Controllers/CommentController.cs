using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoGallery2.Models;
using System.Web.Security.AntiXss;

namespace PhotoGallery2.Controllers
{
    public class CommentController : Controller
    {
        private readonly PhotoDBContext context = new PhotoDBContext();

        public ActionResult Index(int photoID)
        {
            return View(context.Photos.Find(photoID).Comments.ToList());
        }

        public PartialViewResult ListComments(int PhotoID)
        {
            var comments = context.Comments.Where(c => c.PhotoID == PhotoID).ToList();
            return PartialView("_CommentListPartial", comments);
        }

        [HttpGet]
        public PartialViewResult UpdateComment(int PhotoID)
        {
            var comment = new UpdateComment
            {
                PhotoID = PhotoID
            };

            return PartialView("_UpdateComment", comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void UpdateComment(int photoID, string comment)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    //var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    string userID = HttpContext.User.Identity.GetUserId();

                    context.Comments.Add(new Comment
                    {
                        Description = AntiXssEncoder.HtmlEncode(Server.HtmlEncode(comment), false),
                        PhotoID = photoID,
                        UserID = userID,
                    });
                    context.SaveChanges();
                }
            }
        }

        [HttpPost]
        public void DeleteComments(List<int> checkedID)
        {
            foreach (var i in checkedID)
            {
                var comment = context.Comments.Find(i);
                context.Comments.Remove(comment);
            }
            context.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                    context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}