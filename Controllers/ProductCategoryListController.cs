using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("list/productCategorylist")]
    public class ProductCategoryListController : BaseController
    {
        private readonly AppDbContext _context;

        public ProductCategoryListController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }
        private async Task<PageViewModel<ProductCategory>> LoadDataAsync(PaginationFilter pagination, Dictionary<string, string> filters)
        {
            var query = _context.productCategory
                .OrderBy(x => x.SortOrder)
                .AsQueryable();

            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            return await Helpers.PaginateAsync(query, pagination.Page, pagination.PageSize);
        }
        [HttpGet("")]
        public async Task<IActionResult> ProductCategoryList([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                ViewData["SearchName"] = name ?? "";
            }
            return View("~/Views/List/ProductCategoryList.cshtml", result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id không hợp lệ.");

            var cate = await _context.productCategory.FindAsync(id);
            if (cate == null)
                return NotFound("Không tìm thấy.");

            _context.productCategory.Remove(cate);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
        [HttpGet("partialTableProductCategory")]
        public async Task<IActionResult> PartialTableProductCategory([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            return PartialView("~/Views/List/ProductCategoryTable.cshtml", result);
        }
    }
}
