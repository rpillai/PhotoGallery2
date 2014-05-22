using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhotoGallery2.Models;
using WebGrease.Css.Extensions;

namespace PhotoGallery2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AlbumController : Controller
    {
        private readonly PhotoDBContext db = new PhotoDBContext();

        public ActionResult Index()
        {
            var albums = db.Albums.Where( a=> a.Photos.Count > 0) ;

            IEnumerable<AlbumViewModel> model = albums.Select(album => new AlbumViewModel
            {
                AlbumID = album.AlbumID,
                AlbumName = album.Name,
                Description = album.Description,
                PhotoCount = album.Photos.Count,
                KeythumbnailPath = album.Photos.FirstOrDefault() != null ? ServerConstants.PHOTO_THUMBS_ROOT + album.Photos.FirstOrDefault().PhotoPath
                                                                            : "holder.js?160x160",
                DateTaken = album.CreateDate

            });

            return View(model);
        }

        public ActionResult ListAlbums()
        {
            var albums = db.Albums.ToList();
            return View(albums);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }


        public ActionResult Create()
        {
            return View();
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
                db.Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("ListAlbums");
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
            Album album = db.Albums.Find(id);
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
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
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
            Album album = db.Albums.Find(id);
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
            var album = db.Albums.Find(id);

            deleteAlbumDirectory(Server.MapPath(ServerConstants.PHOTO_ROOT + "/" + id));

            deleteAlbumDirectory(Server.MapPath(ServerConstants.PHOTO_THUMBS_ROOT + "/" + id));
            
            db.Albums.Remove(album);
            db.SaveChanges();

            return RedirectToAction("ListAlbums");
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
