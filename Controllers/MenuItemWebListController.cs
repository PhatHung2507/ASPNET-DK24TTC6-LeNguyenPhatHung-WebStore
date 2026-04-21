using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebStore.Controllers
{
    [Route("system/menuitemweblist")]
    public class MenuItemWebListController : BaseController
    {
        private readonly AppDbContext _context;

        public MenuItemWebListController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }
        private async Task<PageViewModel<MenuItemWeb>> LoadDataAsync(PaginationFilter pagination, Dictionary<string, string> filters)
        {
            var query = _context.menuItemWeb
                .Include(x => x.MenuCategoryWeb)
                .OrderBy(x => x.SortOrder)
                .AsQueryable();

            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if (filters.TryGetValue("CategoryId", out var categoryId) && !string.IsNullOrWhiteSpace(categoryId))
            {
                query = query.Where(x => x.MenuCategoryWebId.Contains(categoryId));
            }
            return await Helpers.PaginateAsync(query, pagination.Page, pagination.PageSize);
        }

        [HttpGet("")]
        public async Task<IActionResult> MenuitemWebList([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                ViewData["SearchName"] = name ?? "";
            }
            string? selectedCategoryId = null;
            if (filters.TryGetValue("CategoryId", out var catIdStr))
            {
                selectedCategoryId = catIdStr;
            }
            var menuCategorys = _context.menuCategoryWeb.ToList(); 
            ViewBag.MenuCategoryList = new SelectList(menuCategorys, "Id", "Name", selectedCategoryId);
            return View("~/Views/System/MenuitemWebList.cshtml", result);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id không hợp lệ.");

            var menu = await _context.menuItemWeb.FindAsync(id);
            if (menu == null)
                return NotFound("Không tìm thấy menu.");

            _context.menuItemWeb.Remove(menu);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
        [HttpGet("partialTableMenuitemWeb")]
        public async Task<IActionResult> PartialTableMenuitemWeb([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);

            return PartialView("~/Views/System/MenuitemWebTable.cshtml", result);
        }
    }
}
