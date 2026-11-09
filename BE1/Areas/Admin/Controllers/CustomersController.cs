using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE1.Models;

namespace BE1.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // GET: Admin/Customers
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }
        public ActionResult Index1()
        {
            return View(db.Customers.ToList());
        }
        // GET: Admin/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            global::BE1.Models.Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }
        // GET: Admin/Customers/Detail1/5
        public ActionResult Details1(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy thông tin khách hàng đồng thời Include (gộp) danh sách đơn hàng của khách đó
            var customer = db.Customers
                             .Include(c => c.Orders)
                             .FirstOrDefault(c => c.CustomerID == id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // GET: Admin/Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username,IsActive,Role,Password")] global::BE1.Models.Customer customer, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh đại diện mới khi tạo tài khoản
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    ImageFile.SaveAs(path);
                    customer.CustomerImage = "/Images/" + fileName;
                }

                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            global::BE1.Models.Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username,CustomerImage,IsActive,Role,Password")] global::BE1.Models.Customer customer, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = db.Customers.Find(customer.CustomerID);
                if (existingCustomer != null)
                {
                    existingCustomer.CustomerName = customer.CustomerName;
                    existingCustomer.CustomerPhone = customer.CustomerPhone;
                    existingCustomer.CustomerEmail = customer.CustomerEmail;
                    existingCustomer.CustomerAddress = customer.CustomerAddress;
                    existingCustomer.Username = customer.Username;
                    existingCustomer.Password = customer.Password;
                    existingCustomer.Role = customer.Role;
                    existingCustomer.IsActive = customer.IsActive;

                    // Nếu có chọn file ảnh mới thì lưu ảnh mới, nếu không chọn giữ nguyên đường dẫn ảnh cũ
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(ImageFile.FileName);
                        string path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        ImageFile.SaveAs(path);
                        existingCustomer.CustomerImage = "/Images/" + fileName;
                    }

                    db.Entry(existingCustomer).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            global::BE1.Models.Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            global::BE1.Models.Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
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