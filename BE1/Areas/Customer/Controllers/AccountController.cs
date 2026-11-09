using System;
using System.Linq;
using System.Web.Mvc;
using BE1.Models;

namespace BE1.Areas.Customer.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // GET: Customer/Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Customer/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(BE1.Models.Customer model, string returnUrl)
        {
            // Kiểm tra đăng nhập bằng Email và Password
            var user = db.Customers.FirstOrDefault(u => u.CustomerEmail == model.CustomerEmail && u.Password == model.Password);

            if (user != null)
            {
                if (user.IsActive == false)
                {
                    ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa.");
                    return View(model);
                }

                Session["UserID"] = user.CustomerID;
                Session["Username"] = user.Username;
                Session["FullName"] = user.CustomerName;

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("ProductList", "Home1", new { area = "Customer" });
            }

            ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
            return View(model);
        }

        // GET: Customer/Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Customer/Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(BE1.Models.Customer model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra Email đã tồn tại chưa
                var checkEmail = db.Customers.FirstOrDefault(u => u.CustomerEmail == model.CustomerEmail);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("CustomerEmail", "Email này đã được sử dụng.");
                    return View(model);
                }

                // Kiểm tra Username đã tồn tại chưa
                var checkUser = db.Customers.FirstOrDefault(u => u.Username == model.Username);
                if (checkUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại.");
                    return View(model);
                }

                model.Role = "Customer";
                model.IsActive = true;

                db.Customers.Add(model);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }
            return View(model);
        }

        // GET: Customer/Account/Edit
        public ActionResult Edit()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            int id = (int)Session["UserID"];
            var user = db.Customers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: Customer/Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BE1.Models.Customer model)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            if (ModelState.IsValid)
            {
                int userId = (int)Session["UserID"];
                var customer = db.Customers.Find(userId);

                if (customer != null)
                {
                    customer.CustomerName = model.CustomerName;
                    customer.CustomerPhone = model.CustomerPhone;
                    customer.CustomerAddress = model.CustomerAddress;
                    customer.Password = model.Password;

                    db.SaveChanges();

                    Session["FullName"] = customer.CustomerName;
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("ProductList", "Home1", new { area = "Customer" });
                }
            }

            return View(model);
        }

        // GET: Customer/Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("ProductList", "Home1", new { area = "Customer" });
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