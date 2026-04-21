using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using static NuGet.Packaging.PackagingConstants;

namespace WebStore.Controllers
{
    public class HomeController : BaseController
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        public IActionResult Index(string? voucherDateFrom, string? voucherDateTo)
        {
            if (string.IsNullOrEmpty(voucherDateFrom))
            {
                var firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                voucherDateFrom = firstDay.ToString("dd/MM/yyyy");
            }

            if (string.IsNullOrEmpty(voucherDateTo))
            {
                var lastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                                           DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                voucherDateTo = lastDay.ToString("dd/MM/yyyy");
            }

            var dateFrom = DateTime.Parse(voucherDateFrom);
            var dateTo = DateTime.Parse(voucherDateTo).Date;

            ViewData["DateFrom"] = dateFrom.ToString("yyyy-MM-dd");
            ViewData["DateTo"] = dateTo.ToString("yyyy-MM-dd");

            // Tổng doanh thu
            var revenue = (int)(_context.ordersMaster
                .Where(o => o.CustomerPayment.HasValue && o.VoucherDate >= dateFrom && o.VoucherDate <= dateTo)
                .Sum(o => o.CustomerPayment) ?? 0);

            // Tổng đơn
            var totalOrders = _context.ordersMaster
                .Where(o => o.VoucherDate >= dateFrom && o.VoucherDate <= dateTo)
                .Count();

            // Tổng sản phẩm
            var totalProducts = _context.ordersDetail
                .Include(x => x.OrdersMaster)
                .Where(x => x.OrdersMaster != null &&
                            x.OrdersMaster.VoucherDate >= dateFrom &&
                            x.OrdersMaster.VoucherDate <= dateTo)
                .Where(d => d.ProductId != null)
                .Select(d => d.ProductId)
                .Distinct()
                .Count();

            // Tổng khách hàng
            var totalCustomers = _context.ordersMaster
                .Where(o => o.CustomerId != null && o.VoucherDate >= dateFrom && o.VoucherDate <= dateTo)
                .Select(o => o.CustomerId)
                .Distinct()
                .Count();

            var chartData = _context.ordersMaster
            .Where(o => o.VoucherDate >= dateFrom && o.VoucherDate <= dateTo)
            .GroupBy(o => o.VoucherDate.Value.Date)
            .Select(g => new
            {
                Date = g.Key, // DateTime
                CountOrders = g.Count(),
                CountProducts = g
                    .SelectMany(o => o.OrdersDetail)
                    .Select(d => d.ProductId)
                    .Distinct()
                    .Count(),
                CountCustomers = g
                    .Select(o => o.CustomerId)
                    .Where(id => id != null)
                    .Distinct()
                    .Count(),
                Revenue = (int)(g.Sum(o => o.CustomerPayment) ?? 0)
            })
            .AsEnumerable() 
            .OrderBy(x => x.Date)
            .Select(x => new ChartByDateDto
            {
                Date = x.Date.ToString("dd-MM-yyyy"), 
                CountOrders = x.CountOrders,
                CountProducts = x.CountProducts,
                CountCustomers = x.CountCustomers,
                Revenue = x.Revenue
            })
            .ToList();

            var report = new ReportOverviewViewModel
            {
                Revenue = revenue,
                CountOrder = totalOrders,
                CountProduct = totalProducts,
                CountCustomer = totalCustomers,
                ChartDataByDate = chartData
            };

            return View("~/Views/Home/Index.cshtml", report);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
