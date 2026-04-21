using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class Supplier : BaseTable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Tel { get;set; }
        public string? Address { get;set; }
        public string? Email { get;set; }
        public List<ImportStockMaster> ImportStockMaster { get; set; } = new();
    }
}
