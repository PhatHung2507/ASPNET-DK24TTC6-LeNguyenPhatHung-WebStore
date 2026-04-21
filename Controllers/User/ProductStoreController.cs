using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
namespace WebStore.Controllers
{
    public class ProductStoreController : BaseStoreController
    {
        private readonly AppDbContext _context;
        public ProductStoreController(AppDbContext context)
        {
            _context = context;
        }
        public double GetAverageRating(string productId)
        {
            var ratings = _context.productRating
                .Where(r => r.ProductId == productId && r.Active == true && r.Rating.HasValue)
                .Select(r => r.Rating.Value);

            if (!ratings.Any())
                return 0;

            return Math.Round(ratings.Average(), 2); // làm tròn 2 chữ số
        }
        [HttpPost]
        public JsonResult AddToCart(string productId, int quantity)
        {
            try
            {
                var product = _context.product.FirstOrDefault(p => p.Id == productId);
                if (product == null)
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm." });

                var cartJson = CookieHelper.GetCookie(Request, "Cart");
                List<CartItem> cart = string.IsNullOrEmpty(cartJson)
                    ? new List<CartItem>()
                    : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

                var item = cart.FirstOrDefault(x => x.ProductId == productId);
                if (item != null)
                {
                    item.Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Image = product.ImageOutstanding,
                        Price = product.SalePrice ?? 0,
                        Quantity = quantity
                    });
                }

                string updatedCartJson = JsonConvert.SerializeObject(cart);
                CookieHelper.SetCookie(Response, "Cart", updatedCartJson, 7);

                // Tính tổng số lượng
                int cartTotalQuantity = cart.Sum(x => x.Quantity);

                return Json(new
                {
                    success = true,
                    message = "Đã thêm vào giỏ hàng.",
                    cartTotalQuantity = cartTotalQuantity 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult SubmitReview(int rating, string comment, string productId)
        {
            try
            {
                var customerId = HttpContext.Session.GetString("UserCustomerId");
                if (string.IsNullOrEmpty(customerId))
                {
                    return Json(new { success = false, message = "Bạn chưa đăng nhập." });
                }

                var customer = _context.customer.FirstOrDefault(x => x.Id == customerId);
                if (customer == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng." });
                }

                var review = new ProductRating
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = productId,
                    CustomerId = customerId,
                    Rating = rating,
                    Comment = comment,
                    Active = true,
                    DateRating = DateTime.Now
                };

                _context.productRating.Add(review);
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    customerName = customer.Name,
                    rating = rating,
                    comment = comment,
                    date = review.DateRating.ToString()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public async Task<IActionResult> Index(string? id)
        {
            ProductViewModel pr = new ProductViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                pr.Product = _context.product.FirstOrDefault(p => p.Id == id);

                if (pr.Product == null)
                    return NotFound();

                pr.Product.ProductImage = _context.productImage
                    .Where(x => x.ProductId == id)
                    .ToList();

                // Các sản phẩm cùng loại
                var sameProducts = _context.product
                    .Where(x => x.ProductCategoryId == pr.Product.ProductCategoryId && x.Id != id)
                    .ToList();

                var productIds = sameProducts.Select(p => p.Id).ToList();

                // Lấy AverageRating
                var ratings = await _context.productRating
                    .Where(r => productIds.Contains(r.ProductId))
                    .GroupBy(r => r.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        AverageRating = Math.Round(g.Average(r => r.Rating.Value), 2)
                    })
                    .ToListAsync();

                // Lấy số lượng đã bán
                var soldList = await _context.ordersDetail
                    .Where(x => productIds.Contains(x.ProductId))
                    .GroupBy(x => x.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalSold = g.Sum(x => x.Quantity)
                    })
                    .ToListAsync();

                // Gán dữ liệu vào ListProductSame
                pr.ListProductSame = sameProducts.Select(p => new ProductSameViewModel
                {
                    Product = p,
                    SoldQuantity = soldList.FirstOrDefault(x => x.ProductId == p.Id)?.TotalSold ?? 0,
                    AverageRating = ratings.FirstOrDefault(x => x.ProductId == p.Id)?.AverageRating ?? 0
                }).ToList();

                // Dữ liệu chính sản phẩm đang xem
                pr.SoldQuantity = _context.ordersDetail
                    .Where(x => x.ProductId == id)
                    .Sum(x => x.Quantity.Value);

                pr.AverageRating = GetAverageRating(id);

                decimal quantityIn = _context.importStockDetail
                    .Where(x => x.ProductId == id)
                    .Sum(x => (decimal?)x.Quantity) ?? 0;

                decimal quantityOut = _context.ordersDetail
                    .Where(x => x.ProductId == id)
                    .Sum(x => (decimal?)x.Quantity) ?? 0;

                pr.Stock = quantityIn - quantityOut;

                // Ratings breakdown
                pr.Rating1 = _context.productRating.Count(x => x.ProductId == id && x.Rating == 1);
                pr.Rating2 = _context.productRating.Count(x => x.ProductId == id && x.Rating == 2);
                pr.Rating3 = _context.productRating.Count(x => x.ProductId == id && x.Rating == 3);
                pr.Rating4 = _context.productRating.Count(x => x.ProductId == id && x.Rating == 4);
                pr.Rating5 = _context.productRating.Count(x => x.ProductId == id && x.Rating == 5);

                decimal totalRating = pr.Rating1 + pr.Rating2 + pr.Rating3 + pr.Rating4 + pr.Rating5;
                totalRating = totalRating == 0 ? 1 : totalRating;

                pr.Percent1 = pr.Rating1 / totalRating;
                pr.Percent2 = pr.Rating2 / totalRating;
                pr.Percent3 = pr.Rating3 / totalRating;
                pr.Percent4 = pr.Rating4 / totalRating;
                pr.Percent5 = pr.Rating5 / totalRating;

                pr.ListProductRating = _context.productRating
                    .Include(x => x.Customer)
                    .Where(x => x.ProductId == id)
                    .OrderByDescending(x => x.DateRating)
                    .ToList();
            }

            return View("~/Views/User/ProductStore/ProductStore.cshtml", pr);
        }
    }
}
