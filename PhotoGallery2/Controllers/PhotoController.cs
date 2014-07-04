using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Microsoft.SqlServer.Server;
using PhotoGallery2.DAL;
using PhotoGallery2.Models;
using Image = System.Drawing.Image;
using PhotoGallery2.CloudService;

namespace PhotoGallery2.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public PhotoController()
        {
            unitOfWork = new UnitOfWork();
        }

        public PhotoController(PhotoDBContext context)
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index(int AlbumID)
        {
            if (AlbumID != 0)
            {
                var photos = unitOfWork.PhotoRepository.Get(filter: p => p.AlbumID == AlbumID).Select(x => new PhotoViewModel
                                {
                                    PhotoID = x.PhotoID,
                                    Title = x.Title,
                                    Description = x.Description,
                                    ThumbnailPath = x.ThumbnailPath,
                                    PhotoPath = x.PhotoPath
                                });

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
                PagedEntity = unitOfWork.PhotoRepository.Get().OrderBy(x => x.PhotoID).Select(p => new ManagePhotoModel
                {
                    PhotoID = p.PhotoID,
                    Description = p.Description,
                    Title = p.Title,
                    AlbumName = p.Album.Name,
                    ThumpnailPath = p.ThumbnailPath
                }).Take(ServerConstants.PAGE_SIZE).ToList(),

                NumberOfPages = Convert.ToInt32(Math.Ceiling((double)unitOfWork.PhotoRepository.Get().Count() / ServerConstants.PAGE_SIZE))
            };

            ViewBag.Albums = new SelectList(unitOfWork.AlbumRepository.Get(), "AlbumID", "Name");
            ViewBag.NumberOfPages = photos.NumberOfPages;

            return PartialView(photos);
        }

        public PartialViewResult GetNextResult(int currentPageNumber)
        {
            var photos = new PagedDataList<ManagePhotoModel>
            {
                PagedEntity = unitOfWork.PhotoRepository.Get(orderby: q => q.OrderBy(d => d.PhotoID)).Select(p => new ManagePhotoModel
                    {
                        PhotoID = p.PhotoID,
                        Description = p.Description,
                        Title = p.Title,
                        AlbumName = p.Album.Name,
                        ThumpnailPath = p.ThumbnailPath
                    }).Skip(ServerConstants.PAGE_SIZE * (currentPageNumber - 1)).Take(ServerConstants.PAGE_SIZE).ToList(),

                NumberOfPages = Convert.ToInt32(Math.Ceiling((double)unitOfWork.PhotoRepository.Get().Count() / ServerConstants.PAGE_SIZE)),
                CurrentPage = currentPageNumber
            };
            ViewBag.NumberOfPages = photos.NumberOfPages;
            return PartialView("_ListPhotos", photos);
        }

        [HttpPost]
        public async Task<bool> DeletePhotos(List<int> photoIDList)
        {
            bool returnValue;

            if (photoIDList.Count == 0) return false;

            try
            {
                var photoService = new PhotoStorageService();

                var photos = unitOfWork.PhotoRepository.Get().Where( x => photoIDList.Contains(x.PhotoID)).ToList();

                unitOfWork.PhotoRepository.DeleteByList(photos);

                returnValue = await photoService.DeletePhotos(photos);
                 
                unitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException(ex.Message);
            }

            return returnValue;
        }

        public ActionResult Details(int? photoID)
        {
            ViewBag.PhotoID = photoID;

            if (photoID.HasValue)
            {
                var photo = unitOfWork.PhotoRepository.GetByID(photoID);
                return View(photo);
            }
            return View();

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
            var photo = unitOfWork.PhotoRepository.Get(filter: w => w.PhotoID == photoID).FirstOrDefault();

            if (photo != null)
            {
                return View(photo);
            }

            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PhotoID, Title,Description")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                var p = unitOfWork.PhotoRepository.GetByID(photo.PhotoID);

                p.Title = photo.Title;
                p.Description = photo.Description;

            }

            unitOfWork.Save();

            return RedirectToAction("Manage");
        }

        public ActionResult Upload(int? albumID)
        {
            var context = new PhotoDBContext();

            ViewBag.Albums = new SelectList(context.Albums.ToList(), "AlbumID", "Name", albumID);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Upload(FormCollection formData)
        {
            var albumID = Convert.ToInt32(formData["Albums"]);

            for (var i = 0; i < Request.Files.Count; i++)
            {
                var fileBase = Request.Files[i];

                if (fileBase != null)
                {
                    var photo = new Photo
                    {
                        AlbumID = albumID,
                        Description = formData["Description"],
                        ContentType = fileBase.ContentType
                    };

                    var photoService = new PhotoStorageService();

                    await photoService.UploadPhotoAsync(photo, fileBase);

                    unitOfWork.PhotoRepository.Insert(photo);
                    unitOfWork.Save();
                }
            }
            return RedirectToAction("Index", new { AlbumID = albumID });
        }

        public JsonResult GetPhotosForSlideShow(int albumID)
        {
            var photos = unitOfWork.PhotoRepository.Get(filter: q => q.AlbumID == albumID).Select(x => new
                                           {
                                               title = x.Title,
                                               href = x.PhotoPath,
                                               thumbnail = x.ThumbnailPath,
                                               type = x.ContentType
                                           });

            return Json(photos, "data", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNext(int albumID, int photoID, string flag)
        {
            Photo photo;

            if (flag == "N")
            {
                photo =
                    unitOfWork.PhotoRepository.Get(filter: w => w.AlbumID == albumID && w.PhotoID > photoID,
                        @orderby: o => o.OrderBy(q => q.PhotoID)).FirstOrDefault() ??
                    unitOfWork.PhotoRepository.Get(filter: w => w.AlbumID == albumID,
                            @orderby: o => o.OrderBy(q => q.PhotoID)).First();
            }
            else
            {
                photo = unitOfWork.PhotoRepository.Get(@orderby: o => o.OrderByDescending(p => p.PhotoID),
                    filter: w => w.AlbumID == albumID && w.PhotoID < photoID).Take(1).FirstOrDefault() ??
                        unitOfWork.PhotoRepository.Get(filter: p => p.AlbumID == albumID,
                        @orderby: o => o.OrderByDescending(p => p.PhotoID)).First();
            }

            var photoData = new PhotoViewModel
            {
                PhotoID = photo.PhotoID,
                Description = photo.Description,
                PhotoPath = photo.PhotoPath,
                Title = photo.Title
            };

            return Json(photoData, "PhotoData", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void MovePhotos(List<int> selectedPhotos, int albumID)
        {
            foreach (var selectedPhoto in selectedPhotos)
            {
                var photo = unitOfWork.PhotoRepository.GetByID(selectedPhoto);

                if (photo != null)
                {
                    photo.AlbumID = albumID;

                    movePhotoToNewAlbum(photo.PhotoPath, albumID);

                    photo.PhotoPath = albumID + "//" + photo.FileName;
                    unitOfWork.PhotoRepository.Update(photo);
                }
            }

            unitOfWork.Save();

        }

        private void movePhotoToNewAlbum(string currentPath, int newAlbumID)
        {
            //var getPhotoFileName = Path.GetFileName(currentPath);

            //var currentDirectory = ServerConstants.PHOTO_ROOT + currentPath;
            //var newDirectory = ServerConstants.PHOTO_ROOT + newAlbumID;

            

            //currentDirectory = ServerConstants.PHOTO_THUMBS_ROOT + currentPath;
            //newDirectory = ServerConstants.PHOTO_THUMBS_ROOT + newAlbumID;

            //movePhotoAndThumbnailFile(newDirectory, currentDirectory, getPhotoFileName);
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
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}

