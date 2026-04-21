using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Collections.Generic;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebStore.Controllers
{
    [Route("sale/Orderslist")]
    public class OrdersListController : BaseController
    {
        private readonly AppDbContext _context;

        public OrdersListController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }
        private async Task<PageViewModel<OrdersMaster>> LoadDataAsync(PaginationFilter pagination, Dictionary<string, string> filters)
        {
            var query = _context.ordersMaster
                .Include(x => x.Customer)
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
        public async Task<IActionResult> OrdersList([FromQuery] PaginationFilter filter)
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
            return View("~/Views/Sale/OrdersList.cshtml", result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id không hợp lệ.");

            var Orders = await _context.ordersMaster.FindAsync(id);
            if (Orders == null)
                return NotFound("Không tìm thấy.");

            List<OrdersDetail> lstDetail = await _context.ordersDetail.Where(x=>x.OrdersMasterId == id).ToListAsync();
            if(lstDetail.Count > 0)
            {
                foreach (var item in lstDetail)
                {
                    _context.ordersDetail.Remove(item);
                }
            }
            _context.ordersMaster.Remove(Orders);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
        [HttpGet("partialTableOrders")]
        public async Task<IActionResult> PartialTableOrders([FromQuery] PaginationFilter filter)
        {
            var filters = Helpers.ExtractFilters(Request.Query);
            var result = await LoadDataAsync(filter, filters);
            return PartialView("~/Views/Sale/OrdersTable.cshtml", result);
        }
        [HttpGet("print")]
        public async Task<IActionResult> Print(string orderId)
        {
            var ordersMaster = await _context.ordersMaster
            .Include(o => o.Customer)
            .ThenInclude(c => c.Location)
            .ThenInclude(c => c.Ward)
            .FirstOrDefaultAsync(o => o.Id == orderId);

            if (ordersMaster == null)
                return NotFound("Không tìm thấy.");
            List<ProductModel> lstProduct = new List<ProductModel>();
            var ordersDetail = await _context.ordersDetail.Where(x=>x.OrdersMasterId == orderId).ToListAsync();
            foreach (var item in ordersDetail) 
            {
                ProductModel model = new ProductModel();
                Product product = await _context.product.FindAsync(item.ProductId);
                model.Name = product.Name;
                model.Quantity = item.Quantity.Value;
                model.UnitPrice = item.Price.Value;
                lstProduct.Add(model);
            }
            string CurrentAdmin = HttpContext.Session.GetString("UserId") ?? "";
            var document = new ReceiptDocument
            {
                OrderDate = DateTime.Now,
                EmployeeName = CurrentAdmin,
                Products = lstProduct,
                Note = ordersMaster.Note,
                Customer = ordersMaster.Customer.Name,
                Address = ordersMaster.Customer.Address + ", " + ordersMaster.Customer.Ward.Name + ", " + ordersMaster.Customer.Location.Name
            };

            var pdf = document.GeneratePdf();
            return File(pdf, "application/pdf", $"Phieu_{orderId}.pdf");
        }
    }
}
