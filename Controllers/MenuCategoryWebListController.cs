using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("system/menucategoryweblist")]
    public class MenuCategoryWebListController : BaseController
    {
        private readonly AppDbContext _context;

        public MenuCategoryWebListController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }
        private async Task<PageViewModel<MenuCategoryWeb>> LoadDataAsync(PaginationFilter pagination, Dictionary<string, string> filters)
        {
            var query = _context.menuCategoryWeb
                .OrderBy(x => x.SortOrder)
                .AsQueryable();

            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            return await Helpers.PaginateAsync(query, pagination.Page, pagination.PageSize);
        }
        [HttpGet("")]
        public async Task<IActionResult> MenucategoryWebList([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                ViewData["SearchName"] = name ?? "";
            }
            return View("~/Views/System/MenucategoryWebList.cshtml", result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id không hợp lệ.");

            var cate = await _context.menuCategoryWeb.FindAsync(id);
            if (cate == null)
                return NotFound("Không tìm thấy danh mục.");

            _context.menuCategoryWeb.Remove(cate);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
        [HttpGet("partialTableMenucategoryWeb")]
        public async Task<IActionResult> PartialTableMenucategoryWeb([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            return PartialView("~/Views/System/MenucategoryWebTable.cshtml", result);
        }
    }
}
