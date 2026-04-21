using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("system/menucategorywebedit")]
    public class MenucategoryWebEditController : BaseController
    {
        private readonly AppDbContext _context;

        public MenucategoryWebEditController(AppDbContext context, IHttpContextAccessor accessor)
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
                return PartialView("~/Views/System/MenucategoryWebEdit.cshtml", new MenuCategoryWeb());
            }

            // Trường hợp cập nhật: lấy dữ liệu từ DB
            var cate = await _context.menuCategoryWeb.FindAsync(id);
            if (cate == null)
            {
                return NotFound("Không tìm thấy danh mục.");
            }

            return PartialView("~/Views/System/MenucategoryWebEdit.cshtml", cate);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(MenuCategoryWeb model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                // Trường hợp thêm mới, model.Id chưa có
                model.Id = Helpers.ConvertNameToCode(model.Name);
                _context.menuCategoryWeb.Add(model);
            }
            else
            {
                // Trường hợp cập nhật: lấy bản ghi gốc theo Id từ model.Id
                var existingCate = await _context.menuCategoryWeb.FindAsync(model.Id);
                if (existingCate == null)
                {
                    return NotFound("Không tìm thấy danh mục.");
                }

                // Cập nhật các trường ngoại trừ Id
                existingCate.Name = model.Name;
                existingCate.IconClass = model.IconClass;
                existingCate.SortOrder = model.SortOrder;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Lưu thành công" });
        }

    }

}
