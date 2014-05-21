using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoGallery2.Attributes
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if(filterContext.ExceptionHandled ||
                 filterContext.HttpContext.IsCustomErrorEnabled == false )
                return;

                base.OnException(filterContext);
        }
    }
}