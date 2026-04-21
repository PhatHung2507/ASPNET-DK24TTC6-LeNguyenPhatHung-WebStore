using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("list/productedit")]
    public class ProductEditController : BaseController
    {
        private readonly AppDbContext _context;

        public ProductEditController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? id)
        {
            var viewModel = new ProductByCategoryViewModel
            {
                ProductCategorys = await _context.productCategory.ToListAsync(),
                Brands = await _context.brand.ToListAsync()
            };

            if (!string.IsNullOrEmpty(id))
            {
                var item = await _context.product.FindAsync(id);
                if (item == null) return NotFound("Không tìm thấy menu.");
                item.ProductImage = await _context.productImage.Where(x => x.ProductId == id).ToListAsync();
                viewModel.Product = item;
            }

            return PartialView("~/Views/List/ProductEdit.cshtml", viewModel);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(Product model, List<IFormFile> Images, string DeletedImageIds)
        {
            string currentAdmin = HttpContext.Session.GetString("UserId") ?? "";

            Product product;

            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
                model.UserCreate = currentAdmin;
                model.DateCreate = DateTime.Now;
                model.UserUpdate = currentAdmin;
                model.DateUpdate = DateTime.Now;
                product = model;
                _context.product.Add(product);
            }
            else
            {
                product = await _context.product
                    .Include(p => p.ProductImage)
                    .FirstOrDefaultAsync(p => p.Id == model.Id);

                if (product == null)
                    return NotFound("Không tìm thấy sản phẩm.");

                product.Name = model.Name;
                product.Code = model.Code;
                product.ProductCategoryId = model.ProductCategoryId;
                product.Description = model.Description;
                product.PurchasePrice = model.PurchasePrice;
                product.SalePrice = model.SalePrice;
                product.BrandId = model.BrandId;
                product.UserUpdate = currentAdmin;
                product.DateUpdate = DateTime.Now;
            }

            // ✅ Xoá ảnh nếu có
            if (!string.IsNullOrEmpty(DeletedImageIds))
            {
                var idsToDelete = DeletedImageIds.Split(',').ToList();
                var imagesToDelete = await _context.productImage
                    .Where(p => idsToDelete.Contains(p.Id)).ToListAsync();

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
            }

            // ✅ Thêm ảnh mới
            if (Images != null && Images.Count > 0)
            {
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                Directory.CreateDirectory(uploadFolder);

                foreach (var file in Images)
                {
                    if (file.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var productImage = new ProductImage
                        {
                            Id = Guid.NewGuid().ToString(),
                            ProductId = product.Id,
                            Url = $"/images/products/{fileName}"
                        };

                        _context.productImage.Add(productImage);
                    }
                }

            }
            if (product.ProductImage != null && product.ProductImage.Any())
            {
                product.ImageOutstanding = product.ProductImage.First().Url;
            }
            else
            {
                product.ImageOutstanding = null;
            }    
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu thành công" });
        }
    }
}
