using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PhotoGallery2.DAL;
using PhotoGallery2.Models;
using WebGrease.Css.Extensions;

namespace PhotoGallery2.Controllers
{
    [Authorize]
    public class AlbumController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public AlbumController()
        {
            unitOfWork = new UnitOfWork(new PhotoDBContext());
        }


        public AlbumController(PhotoDBContext context)
        {
            unitOfWork = new UnitOfWork(context);
        }


        [Authorize]
        public ActionResult Index()
        {
            var albums = unitOfWork.AlbumRepository.Get(filter: a => a.Photos.Count > 0);

            IEnumerable<AlbumViewModel> model = albums.Select(album => new AlbumViewModel
            {
                AlbumID = album.AlbumID,
                AlbumName = album.Name,
                Description = album.Description,
                PhotoCount = album.Photos.Count,
                KeythumbnailPath = album.Photos.FirstOrDefault() != null ? album.Photos.FirstOrDefault().ThumbnailPath
                                                                    : "holder.js?160x160",
                DateTaken = album.CreateDate

            });

            return View(model);
        }

        public ActionResult Manage()
        {
            var albums =  unitOfWork.AlbumRepository.Get();
            return View(albums);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var album = unitOfWork.AlbumRepository.GetByID(id);

            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }


        public ActionResult Create()
        {
            var album = new Album
            {
                CreateDate =  DateTime.Now
            };
            return View(album);
        }

        // POST: /Album/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AlbumID,Name,Description,CreateDate,PlaceTaken")] Album album)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.AlbumRepository.Insert(album);
                unitOfWork.Save();
                return RedirectToAction("Manage");
            }

            return View(album);
        }

        // GET: /Album/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var album = unitOfWork.AlbumRepository.GetByID(id);

            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: /Album/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AlbumID,Name,Description,CreateDate,PlaceTaken")] Album album)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.AlbumRepository.Update(album);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(album);
        }

        // GET: /Album/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var album = unitOfWork.AlbumRepository.GetByID(id);

            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: /Album/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            unitOfWork.AlbumRepository.Delete(id);

            deleteAlbumDirectory(Server.MapPath(ServerConstants.PHOTO_ROOT + "/" + id));

            deleteAlbumDirectory(Server.MapPath(ServerConstants.PHOTO_THUMBS_ROOT + "/" + id));
            
            unitOfWork.Save();

            return RedirectToAction("Manage");
        }

        private void deleteAlbumDirectory(string albumPath)
        {
            var dirInfo = new DirectoryInfo(albumPath);
            if (dirInfo.Exists)
                dirInfo.Delete(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
