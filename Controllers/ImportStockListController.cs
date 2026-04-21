using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebStore.Controllers
{
    [Route("sale/ImportStocklist")]
    public class ImportStockListController : BaseController
    {
        private readonly AppDbContext _context;

        public ImportStockListController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }
        private async Task<PageViewModel<ImportStockMaster>> LoadDataAsync(PaginationFilter pagination, Dictionary<string, string> filters)
        {
            var query = _context.importStockMaster
                .Include(x => x.Supplier)
                .OrderBy(x => x.VoucherDate)
                .AsQueryable();

            if (filters.TryGetValue("voucherDateFrom", out string voucherDateFrom) && !string.IsNullOrWhiteSpace(voucherDateFrom))
            {
                query = query.Where(x => x.VoucherDate >= DateTime.Parse(voucherDateFrom));
            }
            if (filters.TryGetValue("voucherDateTo", out string voucherDateTo) && !string.IsNullOrWhiteSpace(voucherDateTo))
            {
                query = query.Where(x => x.VoucherDate <= DateTime.Parse(voucherDateTo));
            }
            return await Helpers.PaginateAsync(query, pagination.Page, pagination.PageSize);
        }
        [HttpGet("")]
        public async Task<IActionResult> ImportStockList([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            if (filters.TryGetValue("voucherDateFrom", out string voucherDateFrom) && !string.IsNullOrWhiteSpace(voucherDateFrom))
            {
                ViewData["VoucherDateFrom"] = DateTime.Parse(voucherDateFrom).ToString("dd/MM/yyyy") ?? "";
            }
            if (filters.TryGetValue("voucherDateTo", out string voucherDateTo) && !string.IsNullOrWhiteSpace(voucherDateTo))
            {
                ViewData["VoucherDateTo"] = DateTime.Parse(voucherDateTo).ToString("dd/MM/yyyy") ?? "";
            }
            return View("~/Views/Sale/ImportStockList.cshtml", result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id không hợp lệ.");

            var ImportStock = await _context.importStockMaster.FindAsync(id);
            if (ImportStock == null)
                return NotFound("Không tìm thấy.");

            List<ImportStockDetail> lstDetail = await _context.importStockDetail.Where(x=>x.ImportStockMasterId == id).ToListAsync();
            if(lstDetail.Count > 0)
            {
                foreach (var item in lstDetail)
                {
                    _context.importStockDetail.Remove(item);
                }
            }
            _context.importStockMaster.Remove(ImportStock);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
        [HttpGet("partialTableImportStock")]
        public async Task<IActionResult> PartialTableImportStock([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            return PartialView("~/Views/Sale/ImportStockTable.cshtml", result);
        }
    }
}
