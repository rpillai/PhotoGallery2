using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PhotoGallery2.Models;
using WebGrease.Css.Extensions;

namespace PhotoGallery2.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        readonly PhotoDBContext context = new PhotoDBContext();
        
        public ActionResult Index(int AlbumID)
        {
            if (AlbumID != 0)
            {
                var photos = context.Photos.Where(p => p.AlbumID == AlbumID);
                photos.ForEach(x => x.PhotoPath = @"~//Photos//thumbs//" + x.PhotoPath);
                return View(photos);
            }

            return RedirectToAction("Index", "Album");
        }

<<<<<<< HEAD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumID"></param>
        /// <returns></returns>
        public ActionResult Edit(int photoID)
        {
            var photo = context.Photos.Find(new { PhotoID = photoID});

            if (photo != null)
            {
                return View(photo);
            }

            return RedirectToAction("ListPhotos");

        }
=======
>>>>>>> 47fdbd099660bff83ad7c3586c55ede34306fe6d

        public ActionResult Upload()
        {
            ViewBag.Albums = new SelectList(context.Albums.ToList(),"AlbumID","Name");
            return View();
        }

<<<<<<< HEAD
        public ActionResult ListPhotos()
        {
            var photos = context.Photos.ToList();
            return View(photos);
        }

=======
>>>>>>> 47fdbd099660bff83ad7c3586c55ede34306fe6d
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Administrator")]
        public ActionResult Upload(FormCollection formData)
        {
            var albumDirectory = Server.MapPath(ServerConstants.PHOTO_ROOT);
            var albumID = Convert.ToInt32(formData["Albums"]);

            albumDirectory += albumID + "/";
            
            for(var i=0;i<Request.Files.Count;i++)
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
                        PhotoPath =  albumID + "\\" + fileBase.FileName
                    };

                    saveAndCreateThumbnail(albumDirectory, albumID, fileBase);

                    context.Photos.Add(photo);
                    context.SaveChanges();    
                }
            }
            
<<<<<<< HEAD
            return RedirectToAction("Index", new { AlbumID = albumID });
=======
            return RedirectToAction("Index");
>>>>>>> 47fdbd099660bff83ad7c3586c55ede34306fe6d
        }

        private void saveAndCreateThumbnail(string albumDirectory,int albumID, HttpPostedFileBase fileBase)
        {
            var albumThumbnailDirectory = Server.MapPath(ServerConstants.PHOTO_THUMBS_ROOT) + albumID + "\\";

            if (checkAndCreateDirectory(albumDirectory))
            {
                fileBase.SaveAs(albumDirectory + "/" + fileBase.FileName);    
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
                catch (Exception){
                }
            }

            return created;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
                context.Dispose();

            base.Dispose(disposing);
        }
    }
}