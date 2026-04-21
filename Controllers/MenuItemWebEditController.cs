using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("system/menuitemwebedit")]
    public class MenuitemWebEditController : BaseController
    {
        private readonly AppDbContext _context;

        public MenuitemWebEditController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? id)
        {
            var viewModel = new MenuItemByCategoryViewModel
            {
                MenuCategories = await _context.menuCategoryWeb.ToListAsync()
            };

            if (!string.IsNullOrEmpty(id))
            {
                var item = await _context.menuItemWeb.FindAsync(id);
                if (item == null) return NotFound("Không tìm thấy menu.");

                viewModel.MenuItemWeb = item;
            }

            return PartialView("~/Views/System/MenuitemWebEdit.cshtml", viewModel);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(MenuItemWeb model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                // Trường hợp thêm mới, model.Id chưa có
                model.Id = Helpers.ConvertNameToCode(model.Name);
                _context.menuItemWeb.Add(model);
            }
            else
            {
                // Trường hợp cập nhật: lấy bản ghi gốc theo Id từ model.Id
                var existingMenu = await _context.menuItemWeb.FindAsync(model.Id);
                if (existingMenu == null)
                {
                    return NotFound("Không tìm thấy danh mục.");
                }

                // Cập nhật các trường ngoại trừ Id
                existingMenu.Name = model.Name;
                existingMenu.Url = model.Url;
                existingMenu.EditUrl = model.EditUrl;
                existingMenu.SortOrder = model.SortOrder;
                existingMenu.MenuCategoryWebId = model.MenuCategoryWebId;
                existingMenu.See = model.See;
                existingMenu.New = model.New;
                existingMenu.Edit = model.Edit;
                existingMenu.Remove = model.Remove;
                existingMenu.Visible = model.Visible == true ? true : false;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    var a = ex.InnerException.Message;
                }
            }

            return Ok(new { message = "Lưu thành công" });
        }

    }

}
