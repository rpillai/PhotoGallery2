using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoGallery2.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        PhotoGallery2.Models.PhotoDBContext context = new Models.PhotoDBContext();
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }


        public PartialViewResult GetComments(int photoID)
        {
            var comments = context.Comments.Select(c => c).Where(x => x.PhotoID == photoID);

            return PartialView(comments);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                context.Dispose();

            base.Dispose(disposing);
        }
    }
}