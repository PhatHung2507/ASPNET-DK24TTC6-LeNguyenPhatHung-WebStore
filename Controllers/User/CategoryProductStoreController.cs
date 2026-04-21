using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Models;
using WebStore.Models.Entities;

namespace WebStore.Controllers
{
    public class CategoryProductStoreController : BaseStoreController
    {
        private readonly AppDbContext _context;
        public CategoryProductStoreController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? type)
        {
            // 1. Truy vấn danh sách sản phẩm theo category (nếu có)
            IQueryable<Product> query = _context.product;

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(p => p.ProductCategoryId == type);

                var category = await _context.productCategory.FirstOrDefaultAsync(c => c.Id == type);
                if (category != null)
                {
                    ViewBag.Tile = category.Name;
                }
            }

            var productList = await query.ToListAsync();

            var productIds = productList.Select(p => p.Id).ToList();

            // 2. Lấy rating trung bình
            var ratings = await _context.productRating
                .Where(r => productIds.Contains(r.ProductId))
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    AverageRating = Math.Round(g.Average(r => r.Rating.Value), 2)
                })
                .ToListAsync();

            // 3. Lấy số lượng đã bán

            var exportStock = await _context.ordersDetail
                .Where(x => productIds.Contains(x.ProductId))
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalOut = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            // 4. Map sang ProductViewModel
            var productVMList = productList.Select(p =>
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

            // 5. Trả về view
            return View("~/Views/User/CategoryProductStore/CategoryProductStore.cshtml", productVMList);
        }

        [HttpGet]
        public async Task<IActionResult> FilterByPrice(decimal minPrice, decimal maxPrice, string? type)
        {
            // 1. Truy vấn danh sách sản phẩm phù hợp
            var productsQuery = _context.product.AsQueryable();

            if (!string.IsNullOrEmpty(type))
            {
                productsQuery = productsQuery.Where(p => p.ProductCategoryId == type);
            }

            productsQuery = productsQuery.Where(p => p.SalePrice >= minPrice && p.SalePrice <= maxPrice);

            var products = await productsQuery.ToListAsync();
            var productIds = products.Select(p => p.Id).ToList();

            // 2. Lấy rating trung bình
            var ratings = await _context.productRating
                .Where(r => productIds.Contains(r.ProductId))
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    AverageRating = Math.Round(g.Average(r => r.Rating.Value), 2)
                })
                .ToListAsync();

            // 3. Lấy số lượng đã bán
            var exportStock = await _context.ordersDetail
                .Where(x => productIds.Contains(x.ProductId))
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalOut = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            // 4. Trả về kết quả JSON
            var result = products.Select(p =>
            {
                var rating = ratings.FirstOrDefault(r => r.ProductId == p.Id)?.AverageRating ?? 0;
                var sold = exportStock.FirstOrDefault(o => o.ProductId == p.Id)?.TotalOut ?? 0;

                return new
                {
                    p.Id,
                    p.Name,
                    p.SalePrice,
                    p.ImageOutstanding,
                    AverageRating = rating,
                    SoldQuantity = sold
                };
            });

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> SortProductsJson(string sortOrder, string? type)
        {
            // 1. Lọc sản phẩm theo danh mục nếu có
            var productsQuery = _context.product.AsQueryable();

            if (!string.IsNullOrEmpty(type))
            {
                productsQuery = productsQuery.Where(p => p.ProductCategoryId == type);
            }

            // 2. Sắp xếp theo giá
            productsQuery = sortOrder switch
            {
                "desc" => productsQuery.OrderByDescending(p => p.SalePrice),
                "asc" => productsQuery.OrderBy(p => p.SalePrice),
                _ => productsQuery
            };

            var products = await productsQuery.ToListAsync();
            var productIds = products.Select(p => p.Id).ToList();

            // 3. Tính AverageRating
            var ratings = await _context.productRating
                .Where(r => r.Active == true && r.Rating.HasValue && productIds.Contains(r.ProductId))
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    AverageRating = Math.Round(g.Average(r => r.Rating.Value), 2)
                })
                .ToListAsync();

            // 4. Tính tồn kho
            var exportStock = await _context.ordersDetail
                .Where(x => productIds.Contains(x.ProductId))
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalOut = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            // 5. Gộp dữ liệu và trả JSON
            var result = products.Select(p =>
            {
                var rating = ratings.FirstOrDefault(r => r.ProductId == p.Id)?.AverageRating ?? 0;
                var quantityOut = exportStock.FirstOrDefault(o => o.ProductId == p.Id)?.TotalOut ?? 0;
                var sold = quantityOut;

                return new
                {
                    p.Id,
                    p.Name,
                    p.SalePrice,
                    p.ImageOutstanding,
                    AverageRating = rating,
                    SoldQuantity = sold
                };
            });

            return Json(result);
        }

    }
}
