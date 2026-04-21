using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("system/adminwebedit")]
    public class AdminWebEditController : BaseController
    {
        private readonly AppDbContext _context;
        public AdminWebEditController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? id)
        {
            var viewModel = new AdminWebByPermissionViewModel
            {
                Permissions = await _context.permission.ToListAsync()
            };
            if (!string.IsNullOrEmpty(id))
            {
                var item = await _context.adminWeb.FindAsync(id);
                if (item == null) return NotFound("Không tìm thấy nhân viên.");

                viewModel.AdminWeb = item;
            }
            return PartialView("~/Views/System/AdminWebEdit.cshtml", viewModel);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(AdminWeb model)
        {
            string CurrentAdmin = HttpContext.Session.GetString("UserId") ?? "";
            if (string.IsNullOrEmpty(model.Id))
            {
                // Trường hợp thêm mới, model.Id chưa có
                model.Id = Helpers.ConvertNameToCode(model.Name);
                model.Password = Helpers.ToMd5(model.Password);
                model.AdminYn = model.PermissionId == null ? true : false;
                model.UserCreate = CurrentAdmin;
                model.DateCreate = DateTime.Now;
                model.UserUpdate = CurrentAdmin;
                model.DateUpdate = DateTime.Now;
                _context.adminWeb.Add(model);
            }
            else
            {
                // Trường hợp cập nhật: lấy bản ghi gốc theo Id từ model.Id
                var existingAdmin = await _context.adminWeb.FindAsync(model.Id);
                if (existingAdmin == null)
                {
                    return NotFound("Không tìm thấy nhân viên.");
                }
                if(existingAdmin.Password != model.Password)
                {
                    model.Password = Helpers.ToMd5(model.Password);
                }    
                // Cập nhật các trường ngoại trừ Id
                existingAdmin.Name = model.Name;
                existingAdmin.Password = model.Password;
                existingAdmin.Phone = model.Phone;
                existingAdmin.PermissionId = model.PermissionId;
                existingAdmin.AdminYn = model.PermissionId == null ? true : false;
                existingAdmin.UserUpdate = CurrentAdmin;
                existingAdmin.DateUpdate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Lưu thành công" });
        }
    }
}
