using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Models;
using WebStore.Models.Entities;

namespace WebStore.Controllers
{
    public class LoginStoreController : BaseStoreController
    {
        private readonly AppDbContext _context;
        public LoginStoreController(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        [HttpPost]
        public JsonResult Login(string Username, string Password)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
            }

            var user = _context.customer.FirstOrDefault(u => u.Tel == Username && u.Password == Password); 

            if (user != null)
            {
                HttpContext.Session.SetString("UserCustomerTel", user.Tel);
                HttpContext.Session.SetString("UserCustomerId", user.Id);
                HttpContext.Session.SetString("UserCustomerName", user.Name);
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Sai tên đăng nhập hoặc mật khẩu." });
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        [HttpPost]
        public JsonResult Register(string Username,string CustomerName, string Password, string ConfirmPassword)
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
            }

            if (Password != ConfirmPassword)
            {
                return Json(new { success = false, message = "Mật khẩu xác nhận không khớp." });
            }

			if (_context.customer.Any(u => u.Tel == Username))
            {
                return Json(new { success = false, message = "Tên đăng nhập đã tồn tại." });
            }

            _context.customer.Add(new Customer
            {
                Id = Guid.NewGuid().ToString(),
                Name = CustomerName,
                Tel = Username.Trim(),
                Password = Password.Trim()
            });
            _context.SaveChanges();
            return Json(new { success = true });
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Index", "HomeStore"); 
        }
    }
}
