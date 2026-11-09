using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
namespace BE1.Models.viewmodel
{
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }
        public string PageNumber  { get; set; }
        public string PageSize { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public PagedList.IPagedList<Product> NewProducts { get; set; }
    }
}