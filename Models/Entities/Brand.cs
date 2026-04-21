using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class Brand : BaseTable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Product> Product { get; set; } = new();
    }
}
