using System.Net;
using System.Web;
using System.Web.Mvc;
using BE1.Models.viewmodel;
using BE1.Models;
using PagedList;
using System.Linq;
using System.Data.Entity;

namespace BE1.Areas.Customer.Controllers
{
    public class Home1Controller : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // GET: Customer/Home1/Index
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();

            // Tìm kiếm sản phẩm dựa trên từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                                               p.ProductDecription.Contains(searchTerm) ||
                                               p.Category.CategoryName.Contains(searchTerm));
            }

            // Phân trang
            int pageNumber = page ?? 1;
            int pageSize = 6;

            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(10).ToList();
            model.NewProducts = products.OrderBy(p => p.OrderDetails.Count()).Take(20).ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Customer/Home1/ProductDetails/5
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            var products = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();

            ProductDetailsVM model = new ProductDetailsVM();

            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToPagedList(pageNumber, pageSize);
            model.TopProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(8).ToPagedList(pageNumber, pageSize);

            // Gán số lượng (giá trị estimatedValue trong ViewModel sẽ tự động nhân theo số lượng)
            if (quantity.HasValue && quantity.Value > 0)
            {
                model.quantity = quantity.Value;
            }

            return View(model);
        }

        public ActionResult ProductList(int? categoryId, string searchTerm, string sortOrder, int? page)
        {
            // Truy vấn lấy tất cả sản phẩm kèm theo danh mục
            var products = db.Products.Include(p => p.Category).AsQueryable();

            // 1. Lọc theo danh mục nếu có
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value);
                ViewBag.SelectedCategory = categoryId.Value;
            }

            // 2. Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.ProductName.Contains(searchTerm));
                ViewBag.SearchTerm = searchTerm;
            }

            // 3. Sắp xếp sản phẩm
            ViewBag.CurrentSort = sortOrder;
            ViewBag.PriceSortParam = string.IsNullOrEmpty(sortOrder) ? "price_asc" : "";

            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                default:
                    products = products.OrderByDescending(p => p.ProductID); // Mặc định sản phẩm mới nhất lên đầu
                    break;
            }

            // Truyền danh sách Categories ra View để hiển thị bộ lọc bên sidebar
            ViewBag.Categories = db.Categories.ToList();

            int pageSize = 9; // Hiển thị 9 sản phẩm mỗi trang
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
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