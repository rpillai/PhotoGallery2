using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoGallery2.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult FileNotFound(string url)
        {
            ViewBag.ErrorMessage = Request.QueryString["aspxerrorpath"];
            return View("FileNotFound");
        }

        public ActionResult DirectortListing()
        {
            return new HttpNotFoundResult();
        }
    }
}