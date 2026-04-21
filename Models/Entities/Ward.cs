namespace WebStore.Models.Entities
{
    public class Ward
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public string LocationId { get; set; }
        public List<Customer> Customer { get; set; } = new();
    }
}
