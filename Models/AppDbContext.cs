using Microsoft.EntityFrameworkCore;
using WebStore.Models.Entities;

namespace WebStore.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<MenuCategoryWeb> menuCategoryWeb { get; set; }
        public DbSet<MenuItemWeb> menuItemWeb { get; set; }
        public DbSet<AdminWeb> adminWeb { get; set; }
        public DbSet<Permission> permission { get; set; }
        public DbSet<PermissionMenu> permissionMenu { get; set; }
        public DbSet<Supplier> supplier { get; set; }
        public DbSet<Brand> brand { get; set; }
        public DbSet<ProductCategory> productCategory { get; set; }
        public DbSet<Customer> customer { get; set; }
        public DbSet<Location> location { get; set; }
        public DbSet<Ward> ward { get; set; }
        public DbSet<Product> product { get; set; }
        public DbSet<ProductImage> productImage { get; set; }
        public DbSet<ProductRating> productRating { get; set; }
        public DbSet<ImportStockDetail> importStockDetail { get; set; }
        public DbSet<ImportStockMaster> importStockMaster { get; set; }
        public DbSet<OrdersDetail> ordersDetail { get; set; }
        public DbSet<OrdersMaster> ordersMaster { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminWeb>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ImportStockDetail>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<ImportStockMaster>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<OrdersDetail>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<OrdersMaster>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Location>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Ward>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Customer>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ProductCategory>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Brand>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Supplier>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<MenuCategoryWeb>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<MenuItemWeb>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Permission>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<PermissionMenu>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Product>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ProductImage>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ProductRating>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<MenuItemWeb>()
                .HasOne(m => m.MenuCategoryWeb)
                .WithMany(c => c.MenuItemWeb)
                .HasForeignKey(m => m.MenuCategoryWebId);

            modelBuilder.Entity<ImportStockDetail>()
                .HasOne(m => m.ImportStockMaster)
                .WithMany(c => c.ImportStockDetail)
                .HasForeignKey(m => m.ImportStockMasterId);

            modelBuilder.Entity<OrdersDetail>()
                .HasOne(m => m.OrdersMaster)
                .WithMany(c => c.OrdersDetail)
                .HasForeignKey(m => m.OrdersMasterId);

            modelBuilder.Entity<OrdersDetail>()
                .HasOne(m => m.Product)
                .WithMany(c => c.OrdersDetail)
                .HasForeignKey(m => m.ProductId);

            modelBuilder.Entity<ImportStockDetail>()
                .HasOne(m => m.Product)
                .WithMany(c => c.ImportStockDetail)
                .HasForeignKey(m => m.ProductId);

            modelBuilder.Entity<ProductImage>()
                .HasOne(m => m.Product)
                .WithMany(c => c.ProductImage)
                .HasForeignKey(m => m.ProductId);

            modelBuilder.Entity<ProductRating>()
                .HasOne(m => m.Product)
                .WithMany(c => c.ProductRating)
                .HasForeignKey(m => m.ProductId);

            modelBuilder.Entity<ProductRating>()
                .HasOne(m => m.Customer)
                .WithMany(c => c.ProductRating)
                .HasForeignKey(m => m.CustomerId);

            modelBuilder.Entity<Product>()
                .HasOne(m => m.ProductCategory)
                .WithMany(c => c.Product)
                .HasForeignKey(m => m.ProductCategoryId);

            modelBuilder.Entity<Ward>()
                .HasOne(m => m.Location)
                .WithMany(c => c.Ward)
                .HasForeignKey(m => m.LocationId);

            modelBuilder.Entity<Customer>()
                .HasOne(m => m.Location)
                .WithMany(c => c.Customer)
                .HasForeignKey(m => m.LocationId);

            modelBuilder.Entity<Customer>()
                .HasOne(m => m.Ward)
                .WithMany(c => c.Customer)
                .HasForeignKey(m => m.WardId);

            modelBuilder.Entity<PermissionMenu>()
                .HasOne(m => m.Permission)
                .WithMany(c => c.PermissionMenu)
                .HasForeignKey(m => m.PermissionId);

            modelBuilder.Entity<PermissionMenu>()
                .HasOne(m => m.MenuItemWeb)
                .WithMany(c => c.PermissionMenu)
                .HasForeignKey(m => m.MenuItemWebId);

            modelBuilder.Entity<AdminWeb>()
                .HasOne(m => m.Permission)
                .WithMany(c => c.AdminWeb)
                .HasForeignKey(m => m.PermissionId);

            modelBuilder.Entity<ReportInventoryViewModel>(eb =>
            {
                eb.HasNoKey();        // Model không có khóa chính
                eb.ToView(null);      // Không map tới bảng hay view nào
            });
            modelBuilder.Entity<MenuItemByCategoryViewModel>(eb =>
            {
                eb.HasNoKey();        // Model không có khóa chính
                eb.ToView(null);      // Không map tới bảng hay view nào
            });
            modelBuilder.Entity<AdminWebByPermissionViewModel>(eb =>
            {
                eb.HasNoKey();        // Model không có khóa chính
                eb.ToView(null);      // Không map tới bảng hay view nào
            });
            modelBuilder.Entity<ReportInventoryViewModel>(eb =>
            {
                eb.HasNoKey();        // Model không có khóa chính
                eb.ToView(null);      // Không map tới bảng hay view nào
            });
        }
    }
}
