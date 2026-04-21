using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("list/supplieredit")]
    public class SupplierEditController : BaseController
    {
        private readonly AppDbContext _context;

        public SupplierEditController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Trường hợp tạo mới: trả về form rỗng
                return PartialView("~/Views/List/SupplierEdit.cshtml", new Supplier());
            }

            // Trường hợp cập nhật: lấy dữ liệu từ DB
            var sub = await _context.supplier.FindAsync(id);
            if (sub == null)
            {
                return NotFound("Không tìm thấy");
            }

            return PartialView("~/Views/List/SupplierEdit.cshtml", sub);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(Supplier model)
        {
            string CurrentAdmin = HttpContext.Session.GetString("UserId") ??"";
            if (string.IsNullOrEmpty(model.Id))
            {
                // Trường hợp thêm mới, model.Id chưa có
                model.Id = Helpers.ConvertNameToCode(model.Name);
                model.UserCreate = CurrentAdmin;
                model.DateCreate = DateTime.Now;
                model.UserUpdate = CurrentAdmin;
                model.DateUpdate = DateTime.Now;
                _context.supplier.Add(model);
            }
            else
            {
                // Trường hợp cập nhật: lấy bản ghi gốc theo Id từ model.Id
                var existing = await _context.supplier.FindAsync(model.Id);
                if (existing == null)
                {
                    return NotFound("Không tìm thấy.");
                }
                // Cập nhật các trường ngoại trừ Id
                existing.Name = model.Name;
                existing.Tel = model.Tel;
                existing.Address = model.Address;
                existing.Email = model.Email;
                existing.UserUpdate = CurrentAdmin;
                existing.DateUpdate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu thành công" });
        }

    }

}
