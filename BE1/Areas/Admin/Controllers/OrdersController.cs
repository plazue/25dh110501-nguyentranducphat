using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE1.Models;

namespace BE1.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // GET: Admin/Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Customer);
            return View(orders.ToList());
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Admin/Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            return View();
        }

        // POST: Admin/Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,CustomerID,OrderDate,TotalAmount,PaymentStatus,ShippingAddress,DeliveryMethod,PaymentMethod")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", order.CustomerID);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", order.CustomerID);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,OrderDate,TotalAmount,PaymentStatus,ShippingAddress,DeliveryMethod,PaymentMethod")] Order order, string actionType)
        {
            var existingOrder = db.Orders.Find(order.OrderID);
            if (existingOrder == null)
            {
                return HttpNotFound();
            }

            // Xử lý chuyển đổi trạng thái tuần tự dựa vào actionType từ View
            if (actionType == "Next")
            {
                string currentStatus = existingOrder.PaymentStatus ?? "Chờ xử lý";

                if (currentStatus == "Chờ xử lý")
                {
                    existingOrder.PaymentStatus = "Đã xử lý";
                }
                else if (currentStatus == "Đã xử lý")
                {
                    existingOrder.PaymentStatus = "Đang giao hàng";
                }
                else if (currentStatus == "Đang giao hàng" || currentStatus == "Đang giao")
                {
                    existingOrder.PaymentStatus = "Đã giao";
                }
                else if (currentStatus == "Đã giao")
                {
                    existingOrder.PaymentStatus = "Hoàn thành";
                }
            }
            else if (actionType == "Cancel")
            {
                existingOrder.PaymentStatus = "Đã hủy";
            }
            else
            {
                // Cập nhật thông tin thông thường nếu có
                existingOrder.ShippingAddress = order.ShippingAddress;
                existingOrder.DeliveryMethod = order.DeliveryMethod;
                existingOrder.PaymentMethod = order.PaymentMethod;
            }

            if (ModelState.IsValid)
            {
                db.Entry(existingOrder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", existingOrder.CustomerID);
            return View(existingOrder);
        }

        // GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var order = db.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.OrderID == id);
            if (order != null)
            {
                db.OrderDetails.RemoveRange(order.OrderDetails);
                db.Orders.Remove(order);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
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