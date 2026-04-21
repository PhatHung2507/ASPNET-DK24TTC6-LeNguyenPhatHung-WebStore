using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class Customer : BaseTable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Tel { get;set; }
        public string? Gender { get;set; }
        public DateTime? BirthDay { get;set; }
        public string? Email { get;set; }
        public Location? Location { get; set; }
        public string? LocationId { get; set; }
        public Ward? Ward { get; set; }
        public string? WardId { get; set; }
        public string? Address { get; set; }
        public string? Note { get;set; }
        public string? Password { get;set; }
        public List<ProductRating> ProductRating { get; set; } = new();
    }
    public class CustomerAddressViewModel
    {
        public Customer Customer { get; set; } = new Customer();
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<Ward> Wards { get; set; } = new List<Ward>();
    }
}
