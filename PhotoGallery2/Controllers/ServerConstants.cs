using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace PhotoGallery2.Controllers
{
    public class ServerConstants
    {
        public const string PHOTO_ROOT =  @"~\Photos\";
        public const string PHOTO_THUMBS_ROOT = @"~\Photos\thumbs\";
    }
}