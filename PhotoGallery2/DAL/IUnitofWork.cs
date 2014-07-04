using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhotoGallery2.Models;

namespace PhotoGallery2.DAL
{
    public interface IUnitofWork 
    {
        RepositoryBase<Photo> PhotoRepository { get;  }
        RepositoryBase<Album> AlbumRepository { get;  }

        void Save();
    }
}