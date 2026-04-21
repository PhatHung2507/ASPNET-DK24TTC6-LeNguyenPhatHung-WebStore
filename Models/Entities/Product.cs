using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class Product : BaseTable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public string ProductCategoryId { get; set; }
        public Brand? Brand { get; set; }
        public string? BrandId { get; set; }
        public string? Description {get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public List<ProductImage> ProductImage { get; set; } = new();
        public List<ProductRating> ProductRating { get; set; } = new();
        public List<ImportStockDetail> ImportStockDetail { get; set; } = new();
        public List<OrdersDetail> OrdersDetail { get; set; } = new();
        public string? ImageOutstanding { get; set; }
    }
    public class ProductByCategoryViewModel
    {
        public Product Product { get; set; } = new Product();
        public List<ProductCategory> ProductCategorys { get; set; } = new List<ProductCategory>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
    }
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<ProductSameViewModel> ListProductSame { get;set; } = new List<ProductSameViewModel>();
        public decimal SoldQuantity { get; set; } = 0;
        public double AverageRating { get; set; } = 5;
        public decimal Stock { get; set; } = 0;
        public decimal Rating1 { get; set; } = 0;
        public decimal Rating2 { get; set; } = 0;
        public decimal Rating3 { get; set; } = 0;
        public decimal Rating4 { get; set; } = 0;
        public decimal Rating5 { get; set; } = 0;
        public decimal Percent1 { get; set; } = 0;
        public decimal Percent2 { get; set; } = 0;
        public decimal Percent3 { get; set; } = 0;
        public decimal Percent4 { get; set; } = 0;
        public decimal Percent5 { get; set; } = 0;
        public List<ProductRating> ListProductRating { get; set; } = new List<ProductRating>();
    }
    public class ProductSameViewModel
    {
        public Product Product { get; set; }
        public decimal SoldQuantity { get; set; }
        public double AverageRating { get; set; }
    }
    public class CartItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; }
        public decimal TotalTemp { get; set; }
        public decimal TotalAll { get; set; }
        public Customer Customer { get; set; }
    }
}
