using BE1.Models;
using BE1.Models.viewmodel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BE1.Areas.Admin.Controllers
{
    public class homeController : Controller
    {
        // Sử dụng đúng tên MyStoreEntities1 từ controller Product của bạn
        private MyStoreEntities1 db = new MyStoreEntities1();

        // GET: Admin/home/test
        public ActionResult test()
        {
            var data = db.Categories.Select(c => new ThongKeViewModel
            {
                TenLoai = c.CategoryName,
                SoLuong = c.Products.Count(),
                GiaCaoNhat = c.Products.Any() ? c.Products.Max(p => (decimal?)p.ProductPrice) ?? 0 : 0,
                GiaThapNhat = c.Products.Any() ? c.Products.Min(p => (decimal?)p.ProductPrice) ?? 0 : 0,
                GiaTrungBinh = c.Products.Any() ? c.Products.Average(p => (decimal?)p.ProductPrice) ?? 0 : 0
            }).ToList();

            return View(data);
        }

        // GET: Admin/home/Chart
        public ActionResult Chart()
        {
            var data = db.Categories.Select(c => new ThongKeViewModel
            {
                TenLoai = c.CategoryName,
                SoLuong = c.Products.Count()
            }).ToList();

            return View(data);
        }
    }
}