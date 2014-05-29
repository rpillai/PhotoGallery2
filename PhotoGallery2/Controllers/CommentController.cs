using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PhotoGallery2.Models;

namespace PhotoGallery2.Controllers
{
    public class CommentController : Controller
    {
        private PhotoDBContext context = new PhotoDBContext();

        public PartialViewResult ListComments(int photoID)
        {
            var comments = context.Comments.Where(c => c.PhotoID == photoID).ToList();
            return PartialView("_CommentListPartial", comments);
        }

        public PartialViewResult UpdateComment(int PhotoID)
        {
            var comment = new UpdateComment
            {
                PhotoID = PhotoID
            };

            return PartialView("_UpdateComment", comment);
        }

        [HttpPost]
        public void UpdateComment(UpdateComment formData)
        {
            //var photoID = Request["PhotoID"];
            //var description = Request["Description"];

            //var comment = new UpdateComment
            //{
            //    PhotoID = Convert.ToInt32(formData.PhotoID)
            //};

            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userID = HttpContext.User.Identity.GetUserId();

                    context.Comments.Add(new Comment
                    {
                        Description = formData.Description,
                        PhotoID = Convert.ToInt32(formData.PhotoID),
                        UserID = userID
                    });
                    formData.Description = null;
                    context.SaveChanges();
                }
            }
        }
    }
}