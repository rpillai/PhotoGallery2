using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using PhotoGallery2.Models;

namespace PhotoGallery2.DAL
{
    public class UnitOfWork : IUnitofWork, IDisposable
    {
        private readonly PhotoDBContext context;

        private RepositoryBase<Photo> photoRepository;
        private RepositoryBase<Album> albumRepository;
        private RepositoryBase<Comment> commentRepository; 

        public UnitOfWork()
        {
            this.context = new PhotoDBContext();
        }

        public UnitOfWork(PhotoDBContext context)
        {
            this.context = context;
        }

        public RepositoryBase<Photo> PhotoRepository
        {
            get
            {
                if (this.photoRepository == null)
                    photoRepository = new RepositoryBase<Photo>(context);

                return photoRepository;
            }
        }


        public RepositoryBase<Album> AlbumRepository
        {
            get
            {
                if(this.albumRepository == null)
                    albumRepository = new RepositoryBase<Album>(context);

                return albumRepository;
            }
        }

        public RepositoryBase<Comment> CommentRepository
        {
            get
            {
                if(this.commentRepository == null)
                    commentRepository = new RepositoryBase<Comment>(context);
                return commentRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed == false)
            {
                if (disposing)
                {
                    context.Dispose();    
                }
            }

            disposed = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}