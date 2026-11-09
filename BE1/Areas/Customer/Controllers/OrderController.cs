using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE1.Models;
using BE1.Models.viewmodel;

namespace BE1.Areas.Customer.Controllers
{
    public class OrderController : Controller
    {
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

        // GET: Customer/Order/Checkout
        public ActionResult Checkout()
        {
            var cart = GetSessionCart();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart", new { area = "Customer" });
            }

            // Lấy thông tin khách hàng từ Database dựa vào Session["UserID"] để hiển thị sẵn lên form
            int? customerId = Session["UserID"] as int?;
            if (customerId != null)
            {
                var customer = db.Customers.Find(customerId.Value);
                ViewBag.Customer = customer;
            }

            return View(cart);
        }

        // POST: Customer/Order/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(string fullName, string phone, string address, string note, string paymentMethod, string deliveryMethod)
        {
            var cart = GetSessionCart();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart", new { area = "Customer" });
            }

            try
            {
                // 0. Kiểm tra tồn kho trước khi đặt hàng
                foreach (var item in cart)
                {
                    var productInDb = db.Products.AsNoTracking().FirstOrDefault(p => p.ProductID == item.ProductID);
                    if (productInDb == null || productInDb.Quantity < item.Quantity)
                    {
                        ModelState.AddModelError("", $"Sản phẩm '{item.Title}' không đủ số lượng trong kho (chỉ còn lại {productInDb?.Quantity ?? 0} sản phẩm).");

                        // Truyền lại ViewBag.Customer nếu có lỗi để giữ lại dữ liệu form
                        int? custId = Session["UserID"] as int?;
                        if (custId != null) { ViewBag.Customer = db.Customers.Find(custId.Value); }

                        return View("Checkout", cart);
                    }
                }

                int? customerId = Session["UserID"] as int?;

                // Cập nhật/Lưu Họ tên, Số điện thoại và Địa chỉ mới vào bảng Customer
                if (customerId != null)
                {
                    var customerInDb = db.Customers.Find(customerId.Value);
                    if (customerInDb != null)
                    {
                        customerInDb.CustomerName = fullName;
                        customerInDb.CustomerPhone = phone;
                        customerInDb.CustomerAddress = address;
                    }
                }

                // Tính tổng tiền đơn hàng
                decimal totalAmount = cart.Sum(x => x.Total);

                // 1. Tạo đơn hàng mới khớp với các trường trong bảng Order
                var order = new Order
                {
                    CustomerID = customerId ?? 0,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    PaymentStatus = "Chưa thanh toán",
                    ShippingAddress = address,
                    DeliveryMethod = string.IsNullOrEmpty(deliveryMethod) ? "Giao hàng tiêu chuẩn" : deliveryMethod,
                    PaymentMethod = string.IsNullOrEmpty(paymentMethod) ? "COD" : paymentMethod
                };

                db.Orders.Add(order);
                db.SaveChanges(); // Lưu để lấy OrderID tự sinh

                // 2. Lưu chi tiết vào bảng OrderDetail và cập nhật giảm số lượng tồn kho
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price,
                        TotalPrice = item.Total
                    };
                    db.OrderDetails.Add(orderDetail);

                    db.Database.ExecuteSqlCommand(
                        "UPDATE Product SET Quantity = CASE WHEN Quantity - {0} < 0 THEN 0 ELSE Quantity - {0} END WHERE ProductID = {1}",
                        item.Quantity, item.ProductID
                    );
                }

                db.SaveChanges();

               
                Session["Cart"] = null;

                // 4. Chuyển hướng sang trang thành công
                return RedirectToAction("OrdersSuccess", new { area = "Customer", id = order.OrderID });
            }
            catch (DbEntityValidationException dbEx)
            {
                var errorMessages = dbEx.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => $"[Trường: {x.PropertyName} - Lỗi: {x.ErrorMessage}]");

                string fullErrorMessage = string.Join(" | ", errorMessages);
                ModelState.AddModelError("", "Lỗi Validate dữ liệu: " + fullErrorMessage);

                int? custId = Session["UserID"] as int?;
                if (custId != null) { ViewBag.Customer = db.Customers.Find(custId.Value); }

                return View("Checkout", cart);
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMsg += " -> " + ex.InnerException.Message;
                }

                ModelState.AddModelError("", "Lỗi khi xử lý đơn hàng: " + errorMsg);

                int? custId = Session["UserID"] as int?;
                if (custId != null) { ViewBag.Customer = db.Customers.Find(custId.Value); }

                return View("Checkout", cart);
            }
        }

        // GET: Customer/Order/OrdersSuccess/5
        public ActionResult OrdersSuccess(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ProductList", "Home1", new { area = "Customer" });
            }

            var order = db.Orders.Include(o => o.OrderDetails.Select(d => d.Product)).FirstOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Customer/Order/OrderHistory
        public ActionResult OrderHistory()
        {
            int? customerId = Session["UserID"] as int?;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var orders = db.Orders
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .Where(o => o.CustomerID == customerId.Value)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // GET: Customer/Order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("OrderHistory", "Order", new { area = "Customer" });
            }

            int? customerId = Session["UserID"] as int?;
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var order = db.Orders
                          .Include(o => o.OrderDetails.Select(d => d.Product))
                          .FirstOrDefault(o => o.OrderID == id && o.CustomerID == customerId.Value);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
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