using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoGallery2.Models
{
    public interface IPhotoRepository
    {
        IEnumerable<Photo> GetPhotosByAlbum(int albumID);
        IEnumerable<Photo> GetPhotos();
        Photo GetPhotobyPhotoID(int photoID);
        void DeletePhoto(int photoID);
        void SavePhoto(Photo photo);

        int SaveChanges();
    }
}