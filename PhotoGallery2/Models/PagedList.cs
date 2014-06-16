using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoGallery2.Models
{
    public class PagedDataList<T> where T  : class
    {
        public IEnumerable<T> PagedEntity { get; set; }
        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }


    }
}