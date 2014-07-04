using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoGallery2.DAL;
using PhotoGallery2.Models;
using System.Web.Security.AntiXss;

namespace PhotoGallery2.Controllers
{
    public class CommentController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public CommentController()
        {
            unitOfWork = new UnitOfWork();
        }

        public CommentController(PhotoDBContext context)
        {
            unitOfWork = new UnitOfWork(context);
        }


        public ActionResult Index(int photoID)
        {
            var comments = unitOfWork.CommentRepository.Get().Where(c => c.PhotoID == photoID);
            return View(comments.ToList());
        }

        public PartialViewResult ListComments(int PhotoID)
        {
            var comments = unitOfWork.CommentRepository.Get().Where(c => c.PhotoID == PhotoID).ToList();
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
        public void UpdateComment(int PhotoID, string  Description)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userID = HttpContext.User.Identity.GetUserId();

                    unitOfWork.CommentRepository.Insert(new Comment
                    {
                        Description = AntiXssEncoder.HtmlEncode(Server.HtmlEncode(Description), false),
                        PhotoID = PhotoID,
                        UserID = userID,
                    });
                    unitOfWork.Save();
                }
            }
        }

        [HttpPost]
        public void DeleteComments(List<int> checkedID)
        {
            if (checkedID == null) return;

            var comments = unitOfWork.CommentRepository.Get().Where(c => checkedID.Contains(c.CommentID)).ToList();

            unitOfWork.CommentRepository.DeleteByList(comments);
            unitOfWork.Save();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}