using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PhotoGallery2.Models;


namespace PhotoGallery2.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        readonly PhotoDBContext context = new PhotoDBContext();

        public ActionResult Index(int AlbumID)
        {
            var thumPath = ServerConstants.PHOTO_THUMBS_ROOT;
            var path = ServerConstants.PHOTO_ROOT;

            if (AlbumID != 0)
            {
                var photos = context.Photos.Where(p => p.AlbumID == AlbumID)
                                           .Select(x => new PhotoViewModel
                                           {
                                               PhotoID = x.PhotoID,
                                               Title = x.Title,
                                               Description = x.Description,
                                               ThumbnailPath = thumPath + x.PhotoPath,
                                               PhotoPath =  path + x.PhotoPath
                                           });
                return View(photos);
            }

            return RedirectToAction("Index", "Album");
        }

        public ActionResult ListPhotos()
        {
            var photos = context.Photos.ToList();
            return View(photos);
        }


        public ActionResult Details(int? photoID)
        {
            var photo = context.Photos.Find(photoID);
            photo.PhotoPath = VirtualPathUtility.ToAbsolute(ServerConstants.PHOTO_ROOT + "//" + photo.PhotoPath);
            return View(photo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public ActionResult Edit(int photoID)
        {

            var photo = context.Photos.Find(photoID);

            if (photo != null)
            {
                return View(photo);
            }

            return RedirectToAction("ListPhotos");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Photo photo)
        {
            if (ModelState.IsValid)
            {
                var p = context.Photos.Find(photo.PhotoID);

                p.Title = photo.Title;
                p.Description = photo.Description;

                //context.Entry(p).State = EntityState.Modified;
                context.SaveChanges();
            }

            return RedirectToAction("ListPhotos");
        }

        public ActionResult Upload(int? albumID)
        {

            ViewBag.Albums = new SelectList(context.Albums.ToList(), "AlbumID", "Name",albumID);
            return View();
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Administrator")]
        public ActionResult Upload(FormCollection formData)
        {
            var albumDirectory = Server.MapPath(ServerConstants.PHOTO_ROOT);
            var albumID = Convert.ToInt32(formData["Albums"]);

            albumDirectory += albumID + "/";

            for (var i = 0; i < Request.Files.Count; i++)
            {
                var fileBase = Request.Files[i];

                if (fileBase != null)
                {
                    var photo = new Photo
                    {
                        AlbumID = albumID,
                        Description = formData["Description"],
                        ContentType = fileBase.ContentType,
                        FileName = fileBase.FileName,
                        ContentLength = fileBase.ContentLength,
                        PhotoPath = albumID + "//" + fileBase.FileName
                    };

                    saveAndCreateThumbnail(albumDirectory, albumID, fileBase);

                    context.Photos.Add(photo);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("Index", new { AlbumID = albumID });

        }


        public JsonResult GetPhotosForSlideShow(int albumID)
        {
            var path = VirtualPathUtility.ToAbsolute(ServerConstants.PHOTO_ROOT);
            var thumPath = VirtualPathUtility.ToAbsolute(ServerConstants.PHOTO_THUMBS_ROOT);

            var photos = context.Photos.Where(p => p.AlbumID == albumID)
                                           .Select(x => new 
                                           {
                                               title = x.Title,
                                               href =  path + x.PhotoPath,
                                               thumbnail = thumPath + x.PhotoPath,
                                               type = x.ContentType
                                           });

            foreach (var p in photos)
            {
                Console.WriteLine(p.thumbnail + "     " +  p.href);
            }

            return Json(photos, "data", JsonRequestBehavior.AllowGet);
        }

        private void saveAndCreateThumbnail(string albumDirectory, int albumID, HttpPostedFileBase fileBase)
        {
            var albumThumbnailDirectory = Server.MapPath(ServerConstants.PHOTO_THUMBS_ROOT) + albumID + "\\";

            if (checkAndCreateDirectory(albumDirectory))
            {
                fileBase.SaveAs(albumDirectory + "\\" + fileBase.FileName);
            }


            if (checkAndCreateDirectory(albumThumbnailDirectory))
            {
                var bitMap = Image.FromFile(albumDirectory + "\\" + fileBase.FileName);

                var thumbnail = bitMap.GetThumbnailImage(160, 160, null, IntPtr.Zero);

                thumbnail.Save(albumThumbnailDirectory + fileBase.FileName);

                bitMap.Dispose();
            }
        }

        private bool checkAndCreateDirectory(string directoryName)
        {
            var dirInfo = new DirectoryInfo(directoryName);

            var created = dirInfo.Exists;

            if (created == false)
            {
                try
                {
                    dirInfo.Create();
                    created = true;
                }
                catch (Exception)
                {
                }
            }

            return created;
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                context.Dispose();

            base.Dispose(disposing);
        }
    }
}