using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class OrdersDetail
    {
        public string Id { get; set; }
        public OrdersMaster? OrdersMaster { get; set; }
        public string? OrdersMasterId { get; set; } 
        public Product? Product { get; set; } 
        public string? ProductId { get; set; } 
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalMoney { get; set; }
        public bool? Active { get; set; }
    }
}
