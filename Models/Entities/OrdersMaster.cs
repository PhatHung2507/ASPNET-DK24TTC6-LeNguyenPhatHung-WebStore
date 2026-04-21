using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class OrdersMaster : BaseTable
    {
        public string Id { get; set; }
        public DateTime? VoucherDate { get; set; }
        public Customer? Customer { get; set; }
        public string? CustomerId { get; set; }
        public decimal? TotalMoney { get; set; }
        public decimal? TotalQuantity { get; set; }
        public string? Source { get; set; }
        public string? Status { get; set; }
        public List<OrdersDetail> OrdersDetail { get; set; } = new();
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountMoney { get; set; }
        public decimal? TotalDiscount { get; set; }
        public bool? IsDiscountPercent { get; set; }
        public bool? IsDiscountMoney { get; set; }
        public decimal? MoneyShip { get; set; }
        public decimal? NeedPayment { get; set; }
        public decimal? CustomerPayment { get; set; }
        public string? Note { get; set; }
    }
    public class OrdersMasterViewModel
    {
        public OrdersMaster OrdersMaster { get; set; } = new OrdersMaster();
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<OrdersDetail> OrdersDetails { get; set; } = new List<OrdersDetail>();
    }
    public class InvoiceModel
    {
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public DateTime PrintDate { get; set; }
        public int PrintCount { get; set; }
        public string Employee { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerInfo { get; set; }
        public List<OrderItem> Items { get; set; }
        public string Notes { get; set; }
    }

    public class OrderItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
    //[HttpGet("invoice/{id}")]
    //    public IActionResult Invoice(int id)
    //    {
    //        var model = InvoiceDataService.Get(id); // lấy dữ liệu hóa đơn
    //        var doc = new InvoiceDocument(model);
    //        var pdf = doc.GeneratePdf();  // QuestPDF helper method
    //        return File(pdf, "application/pdf", $"invoice_{model.InvoiceNumber}.pdf");
    //    }
    //}
}