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
using System.Web.Management;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.Ajax.Utilities;
using Microsoft.SqlServer.Server;
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
                                               PhotoPath = path + x.PhotoPath
                                           }).OrderBy(p => p.PhotoID);
                return View(photos);
            }

            return RedirectToAction("Index", "Album");
        }

        public ActionResult Manage()
        {

            return View();
        }

        public PartialViewResult _ListPhotos()
        {
            var photos = new PagedDataList<ManagePhotoModel>
            {
                PagedEntity = context.Photos.OrderBy(x => x.PhotoID).Select(p => new ManagePhotoModel
                {
                    PhotoID = p.PhotoID,
                    Description = p.Description,
                    Title = p.Title,
                    AlbumName = p.Album.Name
                }).Take(5).ToList(),

                NumberOfPages = Convert.ToInt32(Math.Ceiling((double)context.Photos.Count() / 5))
            };

            ViewBag.NumberOfPages = photos.NumberOfPages;

            return PartialView(photos);
        }


        public PartialViewResult GetNextResult(int currentPageNumber, int pageSize)
        {
            var photos = new PagedDataList<ManagePhotoModel>
            {
                PagedEntity = context.Photos.OrderBy(x=> x.PhotoID).Select(p => new ManagePhotoModel
                {
                    PhotoID = p.PhotoID,
                    Description = p.Description,
                    Title = p.Title,
                    AlbumName = p.Album.Name
                }).Skip(pageSize * (currentPageNumber - 1)).Take(pageSize).ToList(),

                NumberOfPages = Convert.ToInt32(Math.Ceiling((double)context.Photos.Count() / 5)),
                CurrentPage = currentPageNumber
            };
            ViewBag.NumberOfPages = photos.NumberOfPages;
            return PartialView("_ListPhotos", photos);
        }



        [HttpPost]
        public void DeletePhotos(List<int> photoIDList)
        {
            if (photoIDList.Count == 0) return;

            foreach (var i in photoIDList)
            {
                var photo = context.Photos.Find(i);

                if (photo == null) continue;

                context.Photos.Remove(photo);

                deletePhotoFile(photo.PhotoPath);
            }
            context.SaveChanges();

        }


        public ActionResult Details(int? photoID)
        {
            ViewBag.PhotoID = photoID;
            var photo = context.Photos.Find(photoID);
            photo.PhotoPath = VirtualPathUtility.ToAbsolute(ServerConstants.PHOTO_ROOT + "//" + photo.PhotoPath);
            return View(photo);
        }

        public PartialViewResult _GetDetailsView(Photo photo)
        {
            return PartialView("_DetailView", photo);
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

            return RedirectToAction("Manage");

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

            return RedirectToAction("Manage");
        }

        public ActionResult Upload(int? albumID)
        {

            ViewBag.Albums = new SelectList(context.Albums.ToList(), "AlbumID", "Name", albumID);
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
                                               href = path + x.PhotoPath,
                                               thumbnail = thumPath + x.PhotoPath,
                                               type = x.ContentType
                                           });

            foreach (var p in photos)
            {
                Console.WriteLine(p.thumbnail + "     " + p.href);
            }

            return Json(photos, "data", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNext(int albumID, int photoID, string flag)
        {
            Photo photo;

            if (flag == "N")
            {
                photo =
                    context.Photos.AsEnumerable().OrderBy(p => p.PhotoID)
                        .Where(p => p.AlbumID == albumID && p.PhotoID > photoID).Take(1).FirstOrDefault();

                if (photo == null)
                    photo = context.Photos.Where(p => p.AlbumID == albumID).OrderBy(p => p.PhotoID).First();
            }
            else
            {
                photo =
                    context.Photos.AsEnumerable().OrderByDescending(p => p.PhotoID)
                        .Where(p => p.AlbumID == albumID && p.PhotoID < photoID).Take(1).FirstOrDefault();

                if (photo == null)
                    photo = context.Photos.Where(p => p.AlbumID == albumID).OrderByDescending(p => p.PhotoID).First();
            }

            photo.PhotoPath = VirtualPathUtility.ToAbsolute(ServerConstants.PHOTO_ROOT + "//" + photo.PhotoPath);

            var photoData = new PhotoViewModel
            {
                PhotoID = photo.PhotoID,
                Description = photo.Description,
                PhotoPath = photo.PhotoPath,
                Title = photo.Title
            };

            return Json(photoData, "PhotoData", JsonRequestBehavior.AllowGet);
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


        private void deletePhotoFile(string filePath)
        {
            var fileInfo = new FileInfo(Path.Combine(Server.MapPath(ServerConstants.PHOTO_ROOT), filePath));

            if (fileInfo.Exists)
                fileInfo.Delete();

            fileInfo = new FileInfo(Path.Combine(Server.MapPath(ServerConstants.PHOTO_THUMBS_ROOT), filePath));

            if (fileInfo.Exists)
                fileInfo.Delete();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                context.Dispose();

            base.Dispose(disposing);
        }
    }
}

