using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class ImportStockDetail 
    {
        public string Id { get; set; }
        public ImportStockMaster? ImportStockMaster { get; set; }
        public string? ImportStockMasterId { get; set; } 
        public Product? Product { get; set; }
        public string? ProductId { get; set; } 
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalMoney { get; set; }
        public bool? Active { get; set; }
    }
}
