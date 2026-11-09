using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BE1.Models;
using BE1.Models.viewmodel; // Khớp với namespace ViewModel của dự án bạn

namespace BE1.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        // Sử dụng chung DbContext của dự án
        private MyStoreEntities1 db = new MyStoreEntities1();

        // Lấy giỏ hàng từ Session
        private List<CartItemViewModel> GetSessionCart()
        {
            var cart = Session["Cart"] as List<CartItemViewModel>;
            if (cart == null)
            {
                cart = new List<CartItemViewModel>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // GET: Customer/Cart/Index
        public ActionResult Index()
        {
            var cart = GetSessionCart();
            return View(cart);
        }

        // POST: Customer/Cart/AddToCart
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return HttpNotFound();
            }

            var cart = GetSessionCart();
            var item = cart.FirstOrDefault(p => p.ProductID == productId);

            // Dự án của bạn dùng ProductPrice làm giá bán chính thức
            decimal finalPrice = product.ProductPrice;

            if (item != null)
            {
                item.Quantity += quantity;
                item.Price = finalPrice;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    ProductID = product.ProductID,
                    Title = product.ProductName, // Khớp với trường ProductName trong model của bạn
                    Price = finalPrice,
                    Image = product.ProductImage,
                    Quantity = quantity
                });
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // POST: Customer/Cart/UpdateQuantity
        [HttpPost]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetSessionCart();
            var item = cart.FirstOrDefault(p => p.ProductID == productId);
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    cart.Remove(item);
                }
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // GET: Customer/Cart/RemoveItem
        public ActionResult RemoveItem(int productId)
        {
            var cart = GetSessionCart();
            var item = cart.FirstOrDefault(p => p.ProductID == productId);
            if (item != null)
            {
                cart.Remove(item);
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // Lấy tổng số lượng sản phẩm hiển thị lên badge giỏ hàng
        public ActionResult CartBadge()
        {
            var cart = Session["Cart"] as List<CartItemViewModel>;
            int totalQty = cart != null ? cart.Sum(x => x.Quantity) : 0;
            return Content($"<span class=\"cart-badge\">{totalQty}</span>");
        }

        // Lấy danh sách sản phẩm thu nhỏ hiển thị khi rê chuột vào icon giỏ hàng
        public ActionResult CartSubmenu()
        {
            var cart = Session["Cart"] as List<CartItemViewModel>;
            return PartialView("_CartSubmenu", cart);
        }
    }
}