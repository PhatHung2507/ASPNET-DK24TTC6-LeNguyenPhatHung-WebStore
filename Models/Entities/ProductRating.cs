using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;

namespace WebStore.Models.Entities
{
    public class ProductRating
    {
        public string Id { get; set; }
        public Product Product { get; set; }
        public string ProductId { get; set; }
        public Customer Customer { get; set; }
        public string CustomerId { get; set; }
        public bool? Active { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? DateRating { get; set; }
    }
}
