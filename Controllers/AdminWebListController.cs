using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("system/adminweblist")]
    public class AdminWebController : BaseController
    {
        private readonly AppDbContext _context;

        public AdminWebController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }
        private async Task<PageViewModel<AdminWeb>> LoadDataAsync(PaginationFilter pagination, Dictionary<string, string> filters)
        {
            var query = _context.adminWeb
                .OrderBy(x => x.Name)
                .AsQueryable();

            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            return await Helpers.PaginateAsync(query, pagination.Page, pagination.PageSize);
        }

        [HttpGet("")]
        public async Task<IActionResult> AdminWebList([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                ViewData["SearchName"] = name ?? "";
            }
            return View("~/Views/System/AdminWebList.cshtml", result);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id không hợp lệ.");

            var admin = await _context.adminWeb.FindAsync(id);
            if (admin == null)
                return NotFound("Không tìm thấy nhân viên.");

            _context.adminWeb.Remove(admin);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
        [HttpGet("partialTableAdminWeb")]
        public async Task<IActionResult> PartialTableAdminWeb([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            return PartialView("~/Views/System/AdminWebTable.cshtml", result);
        }
    }
}
