using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("list/productCategoryedit")]
    public class ProductCategoryEditController : BaseController
    {
        private readonly AppDbContext _context;

        public ProductCategoryEditController(AppDbContext context, IHttpContextAccessor accessor)
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
                return PartialView("~/Views/List/ProductCategoryEdit.cshtml", new ProductCategory());
            }

            // Trường hợp cập nhật: lấy dữ liệu từ DB
            var cate = await _context.productCategory.FindAsync(id);
            if (cate == null)
            {
                return NotFound("Không tìm thấy");
            }

            return PartialView("~/Views/List/ProductCategoryEdit.cshtml", cate);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(ProductCategory model)
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
                _context.productCategory.Add(model);
            }
            else
            {
                // Trường hợp cập nhật: lấy bản ghi gốc theo Id từ model.Id
                var existing = await _context.productCategory.FindAsync(model.Id);
                if (existing == null)
                {
                    return NotFound("Không tìm thấy.");
                }
                // Cập nhật các trường ngoại trừ Id
                existing.Name = model.Name;
                existing.SortOrder = model.SortOrder;
                existing.UserUpdate = CurrentAdmin;
                existing.DateUpdate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu thành công" });
        }
    }
}
