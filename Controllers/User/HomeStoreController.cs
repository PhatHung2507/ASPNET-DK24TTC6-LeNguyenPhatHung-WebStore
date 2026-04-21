using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;

namespace WebStore.Controllers
{
    public class HomeStoreController : BaseStoreController
    {
        private readonly AppDbContext _context;
        public HomeStoreController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // 1. Lấy danh sách sản phẩm nổi bật và bán chạy
            var lstProductOutstanding = await _context.product
                .OrderByDescending(x => x.SalePrice)
                .Take(10)
                .ToListAsync();

            var lstProductBestSelling = await _context.product
                .OrderByDescending(x => x.DateUpdate)
                .Take(10)
                .ToListAsync();

            var productIds = lstProductOutstanding.Select(p => p.Id)
                .Union(lstProductBestSelling.Select(p => p.Id))
                .ToList();

            // 2. Lấy rating trung bình cho các sản phẩm
            var ratings = await _context.productRating
                .Where(r => productIds.Contains(r.ProductId))
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    AverageRating = Math.Round(g.Average(r => r.Rating.Value), 2)
                })
                .ToListAsync();

            // 3. Tính số lượng đã bán

            var exportStock = await _context.ordersDetail
                .Where(x => productIds.Contains(x.ProductId))
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalOut = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            // 4. Gán dữ liệu vào ViewModel
            var productOutstandingVM = lstProductOutstanding.Select(p =>
            {
                var rating = ratings.FirstOrDefault(r => r.ProductId == p.Id)?.AverageRating ?? 0;
                var quantityOut = exportStock.FirstOrDefault(o => o.ProductId == p.Id)?.TotalOut ?? 0;
                var sold = quantityOut;

                return new ProductViewModel
                {
                    Product = p,
                    AverageRating = rating,
                    SoldQuantity = sold
                };
            }).ToList();

            var productBestSellingVM = lstProductBestSelling.Select(p =>
            {
                var rating = ratings.FirstOrDefault(r => r.ProductId == p.Id)?.AverageRating ?? 0;
                var quantityOut = exportStock.FirstOrDefault(o => o.ProductId == p.Id)?.TotalOut ?? 0;
                var sold = quantityOut;

                return new ProductViewModel
                {
                    Product = p,
                    AverageRating = rating,
                    SoldQuantity = sold
                };
            }).ToList();

            // 5. Trả về View
            var model = new HomeStoreViewModel
            {
                ProductOutstanding = productOutstandingVM,
                ProductBestSelling = productBestSellingVM
            };

            return View("~/Views/User/HomeStore/Index.cshtml", model);
        }
    }
}
