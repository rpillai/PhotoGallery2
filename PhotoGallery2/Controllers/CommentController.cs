using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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
            var photo = context.Photos.Find(PhotoID);
            return PartialView("_CommentListPartial", photo.Comments.ToList());
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
        public void UpdateComment(UpdateComment formData)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userID = HttpContext.User.Identity.GetUserId();

                    context.Comments.Add(new Comment
                    {
                        Description = AntiXssEncoder.HtmlEncode(Server.HtmlEncode(formData.Description), false),
                        PhotoID = Convert.ToInt32(formData.PhotoID),
                        UserID = userID
                    });
                    formData.Description = null;
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