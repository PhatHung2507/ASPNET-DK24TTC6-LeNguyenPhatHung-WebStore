using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login");
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string Id, string Password)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(Password))
            {
                TempData["Message"] = "Tên đăng nhập và mật khẩu không được để trống.";
                return View();
            }

            var user = await _context.adminWeb.FindAsync(Id);

            Password = Helpers.ToMd5(Password);
            if (user == null || user.Password != Password)
            {
                TempData["Message"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("Role", user.AdminYn.ToString());
            HttpContext.Session.SetString("PermissionId", user.PermissionId !=null ? user.PermissionId.ToString() : "");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }

}
