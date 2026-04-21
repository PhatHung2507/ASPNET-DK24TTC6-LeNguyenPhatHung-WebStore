using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class ImportStockMaster : BaseTable
    {
        public string Id { get; set; }
        public DateTime? VoucherDate { get; set; }
        public Supplier? Supplier { get; set; }
        public string? SupplierId { get; set; } 
        public decimal? TotalMoney { get; set; }
        public decimal? TotalQuantity { get; set; }
        public List<ImportStockDetail> ImportStockDetail { get; set; } = new();
    }
    public class ImportStockMasterViewModel
    {
        public ImportStockMaster ImportStockMaster { get; set; } = new ImportStockMaster();
        public List<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<ImportStockDetail> ImportStockDetails { get; set; } = new List<ImportStockDetail>();
    }
}
