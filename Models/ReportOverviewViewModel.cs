namespace WebStore.Models
{
    public class ChartByDateDto
    {
        public string Date { get; set; }           // Ngày (yyyy-MM-dd)
        public int CountOrders { get; set; }       // Số đơn trong ngày
        public int CountProducts { get; set; }     // Số sản phẩm bán trong ngày
        public int CountCustomers { get; set; }    // Số khách hàng mới trong ngày
        public int Revenue { get; set; }    // Doanh thu trong ngày
    }
    public class ReportOverviewViewModel
    {
        public int Revenue { get; set; }
        public int CountOrder { get; set; } = 0;
        public int CountProduct { get; set; } = 0;
        public int CountCustomer { get; set; } = 0;
        public List<ChartByDateDto> ChartDataByDate { get; set; } = new();
    }
}
