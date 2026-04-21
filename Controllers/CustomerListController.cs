using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("list/customerlist")]
    public class CustomerListController : BaseController
    {
        private readonly AppDbContext _context;

        public CustomerListController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }
        private async Task<PageViewModel<Customer>> LoadDataAsync(PaginationFilter pagination, Dictionary<string, string> filters)
        {
            var query = _context.customer
                .OrderBy(x => x.Name)
                .AsQueryable();

            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            return await Helpers.PaginateAsync(query, pagination.Page, pagination.PageSize);
        }
        [HttpGet("")]
        public async Task<IActionResult> CustomerList([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                ViewData["SearchName"] = name ?? "";
            }
            return View("~/Views/List/CustomerList.cshtml", result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id không hợp lệ.");

            var cus = await _context.customer.FindAsync(id);
            if (cus == null)
                return NotFound("Không tìm thấy.");

            _context.customer.Remove(cus);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
        [HttpGet("partialTableCustomer")]
        public async Task<IActionResult> PartialTableCustomer([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            return PartialView("~/Views/List/CustomerTable.cshtml", result);
        }
    }
}
