using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class ProductImage : BaseTable
    {
        public string Id { get; set; }
        public Product Product { get; set; }
        public string ProductId { get; set; }
        public string? Url {get; set; }
    }
}
