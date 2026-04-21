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
    [Route("list/productlist")]
    public class ProductListController : BaseController
    {
        private readonly AppDbContext _context;

        public ProductListController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }
        private async Task<PageViewModel<Product>> LoadDataAsync(PaginationFilter pagination, Dictionary<string, string> filters)
        {
            var query = _context.product
                .Include(x => x.ProductCategory)
                .OrderBy(x => x.Code)
                .AsQueryable();

            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if (filters.TryGetValue("code", out var code) && !string.IsNullOrWhiteSpace(code))
            {
                query = query.Where(x => x.Code.Contains(code));
            }
            if (filters.TryGetValue("ProductCategoryId", out var categoryId) && !string.IsNullOrWhiteSpace(categoryId))
            {
                query = query.Where(x => x.ProductCategoryId.Contains(categoryId));
            }
            return await Helpers.PaginateAsync(query, pagination.Page, pagination.PageSize);
        }

        [HttpGet("")]
        public async Task<IActionResult> ProductList([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            if (filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            {
                ViewData["SearchName"] = name ?? "";
            }
            if (filters.TryGetValue("code", out var code) && !string.IsNullOrWhiteSpace(code))
            {
                ViewData["SearchCode"] = code ?? "";
            }
            string? selectedCategoryId = null;
            if (filters.TryGetValue("ProductCategoryId", out var catIdStr))
            {
                selectedCategoryId = catIdStr;
            }
            var cate = _context.productCategory.ToList(); 
            ViewBag.ProductCategoryList = new SelectList(cate, "Id", "Name", selectedCategoryId);
            return View("~/Views/List/ProductList.cshtml", result);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id không hợp lệ.");

            var menu = await _context.product.FindAsync(id);

            if (menu == null)
                return NotFound("Không tìm thấy menu.");

            var imagesToDelete = await _context.productImage
                .Where(p => p.ProductId == id).ToListAsync();

            foreach (var img in imagesToDelete)
            {
                // Xoá file vật lý nếu có
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.Url.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.productImage.Remove(img);
            }
            _context.product.Remove(menu);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
        [HttpGet("partialTableProduct")]
        public async Task<IActionResult> PartialTableProduct([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);

            return PartialView("~/Views/List/ProductTable.cshtml", result);
        }
    }
}
