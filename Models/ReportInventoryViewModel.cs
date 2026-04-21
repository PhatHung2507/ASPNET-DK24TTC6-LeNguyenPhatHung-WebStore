namespace WebStore.Models
{
    public class ReportInventoryViewModel
    {
        public string ProductName { get; set; }
        public int QuantityIn { get; set; } = 0;
        public int QuantityOut { get; set; } = 0;
        public int Inventory { get; set; }
    }
}
