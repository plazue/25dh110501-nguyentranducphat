using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BE1.Models; // Khớp với namespace model của dự án bạn

namespace BE1.Areas.Admin.Controllers
{
    public class InventoryController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // GET: Admin/Inventory/Index
        public ActionResult Index()
        {
            // Lấy danh sách sản phẩm kèm theo thông tin Category, sắp xếp theo ID giảm dần
            var products = db.Products.Include(p => p.Category).OrderByDescending(p => p.ProductID).ToList();
            return View(products);
        }

        // POST: Admin/Inventory/UpdateStock
        [HttpPost]
        public ActionResult UpdateStock(int id, int quantity)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                // Cập nhật số lượng tồn kho trực tiếp vào cột Quantity của bảng Product
                product.Quantity = (product.Quantity) + quantity;
                if (product.Quantity < 0)
                {
                    product.Quantity = 0;
                }

                db.SaveChanges();
                return Json(new { success = true, newStock = product.Quantity });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
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