using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;

namespace WebStore.Controllers
{
    [Route("report/ReportInventory")]
    public class ReportInventoryController : BaseController
    {
        private readonly AppDbContext _context;

        public ReportInventoryController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index(string? productName)
        {
            ViewData["SearchName"] = productName ?? "";
            var report = (
                from product in _context.product
                where string.IsNullOrEmpty(productName) || product.Name.Contains(productName)

                // Left join ImportStockDetail
                join import in _context.importStockDetail on product.Id equals import.ProductId into importGroup
                from import in importGroup.DefaultIfEmpty()

                    // Left join OrdersDetail
                join order in _context.ordersDetail on product.Id equals order.ProductId into orderGroup
                from order in orderGroup.DefaultIfEmpty()

                group new { import, order } by new { product.Id, product.Name } into g

                select new ReportInventoryViewModel
                {
                    ProductName = g.Key.Name,
                    QuantityIn = (int)g.Where(x => x.import != null).Sum(x => x.import.Quantity ?? 0),
                    QuantityOut = (int)g.Where(x => x.order != null).Sum(x => x.order.Quantity ?? 0),
                    Inventory = (int)g.Where(x => x.import != null).Sum(x => x.import.Quantity ?? 0)
                              - (int)g.Where(x => x.order != null).Sum(x => x.order.Quantity ?? 0)
                }
            ).ToList();

            return View("~/Views/Report/ReportInventory.cshtml", report);
        }
    }
}
