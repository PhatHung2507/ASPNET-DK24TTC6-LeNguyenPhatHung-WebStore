namespace WebStore.Models.Entities
{
    public class Location
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Ward> Ward { get; set; } = new();
        public List<Customer> Customer { get; set; } = new();
    }
}
