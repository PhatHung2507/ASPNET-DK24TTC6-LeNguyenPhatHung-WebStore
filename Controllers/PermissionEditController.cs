using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("system/permissionedit")]
    public class PermissionEditController : BaseController
    {
        private readonly AppDbContext _context;

        public PermissionEditController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? id)
        {
            var model = string.IsNullOrEmpty(id)
                ? new Permission()
                : await _context.permission.Include(p => p.PermissionMenu).FirstOrDefaultAsync(p => p.Id == id) ?? new Permission();

            var allMenus = await _context.menuItemWeb
                .Include(m => m.MenuCategoryWeb)
                .OrderBy(m => m.MenuCategoryWeb.SortOrder)
                .ThenBy(m => m.SortOrder)
                .ToListAsync();

            var grouped = allMenus
                .GroupBy(m => m.MenuCategoryWeb.Name)
                .Select(group => new PermissionMenuViewModel
                {
                    CategoryName = group.Key,
                    Items = group.Select(menu => {
                        var assigned = model.PermissionMenu?.FirstOrDefault(p => p.MenuItemWebId == menu.Id);
                        return new MenuItemViewModel
                        {
                            Id = menu.Id,
                            Name = menu.Name,

                            HasSee = menu.See == true,
                            HasNew = menu.New == true,
                            HasEdit = menu.Edit == true,
                            HasRemove = menu.Remove == true,
                            IsSeeChecked = assigned?.See == true,
                            IsNewChecked = assigned?.New == true,
                            IsEditChecked = assigned?.Edit == true,
                            IsRemoveChecked = assigned?.Remove == true
                        };
                    }).ToList()
                }).ToList();

            ViewBag.MenuPermissions = grouped;

            return PartialView("~/Views/System/PermissionEdit.cshtml", model);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(Permission model)
        {
            string CurrentAdmin = HttpContext.Session.GetString("UserId") ?? "";
            // Tạo mới hoặc cập nhật permission
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Helpers.ConvertNameToCode(model.Name);
                model.UserCreate = CurrentAdmin;
                model.DateCreate = DateTime.Now;
                model.UserUpdate = CurrentAdmin;
                model.DateUpdate = DateTime.Now;
                _context.permission.Add(model);
            }
            else
            {
                var existing = await _context.permission.FindAsync(model.Id);
                if (existing == null) return NotFound("Không tìm thấy nhóm quyền.");
                existing.UserUpdate = CurrentAdmin;
                existing.DateUpdate = DateTime.Now;
                existing.Name = model.Name;
            }

            // Xử lý quyền chi tiết
            var oldPermissions = await _context.permissionMenu.Where(p => p.PermissionId == model.Id).ToListAsync();
            _context.permissionMenu.RemoveRange(oldPermissions);
            var permissionData = Request.Form
                .Where(x => x.Key.StartsWith("Permissions["))
                .GroupBy(x => x.Key.Split('[', ']')[1]) // Lấy MenuItemId
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(
                        x => x.Key.Split('.').Last(), // New/Edit/Remove
                        x => true
                    )
                );

            foreach (var item in permissionData)
            {
                _context.permissionMenu.Add(new PermissionMenu
                {
                    Id = Guid.NewGuid().ToString(),
                    PermissionId = model.Id,
                    MenuItemWebId = item.Key,
                    See = item.Value.ContainsKey("See"),
                    New = item.Value.ContainsKey("New"),
                    Edit = item.Value.ContainsKey("Edit"),
                    Remove = item.Value.ContainsKey("Remove")
                });
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu thành công" });
        }

    }

}
