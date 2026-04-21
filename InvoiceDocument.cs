using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using WebStore.Models.Entities;
namespace WebStore
{
    public class ProductModel
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal Total => Quantity * UnitPrice;
    }
    public class ReceiptDocument : IDocument
    {
        public string StoreName { get; set; } = "Omega Store";
        public string StoreAddress { get; set; } = "99 Đường Đồng Hồ, Q.1, TP.HCM";
        public string StorePhone { get; set; } = "0978888673";
        public string EmployeeName { get; set; }
        public string Note { get; set; }
        public string Customer { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public List<ProductModel> Products { get; set; } = new();
        public decimal TotalQuantity => Products.Sum(p => p.Quantity);
        public decimal TotalAmount => Products.Sum(p => p.Total);

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var culture = new CultureInfo("vi-VN");

            container.Page(page =>
            {
                page.Size(PageSizes.A6);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content().Column(column =>
                {
                    column.Spacing(5);

                    // Header
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(StoreName).Bold();
                            col.Item().Text(StoreAddress);
                            col.Item().Text(StorePhone);
                            col.Item().Text($"Nhân viên : {EmployeeName}");
                        });

                        row.ConstantItem(120).AlignRight().Column(col =>
                        {
                            col.Item().Text($"Ngày in : {OrderDate:dd/MM/yyyy}");
                            col.Item().Text($"Giờ in : {OrderDate:HH:mm:ss}");
                        });
                    });

                    column.Item().Container().PaddingVertical(5).Text("ĐƠN HÀNG").Bold().FontSize(16).AlignCenter();

                    column.Item().Text("Khách hàng : " + Customer);
                    column.Item().Text("Địa chỉ nhà : " + Address);

                    // Table header
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(5);  // Tên sản phẩm
                            columns.ConstantColumn(20); // SL
                            columns.ConstantColumn(60); // Đơn giá
                            columns.ConstantColumn(60); // Thành tiền
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Sản phẩm").Bold();
                            header.Cell().AlignRight().Text("SL").Bold();
                            header.Cell().AlignRight().Text("Đơn giá").Bold();
                            header.Cell().AlignRight().Text("Thành tiền").Bold();
                        });

                        foreach (var p in Products)
                        {
                            table.Cell().Text(p.Name);
                            table.Cell().AlignRight().Text(p.Quantity.ToString());
                            table.Cell().AlignRight().Text(p.UnitPrice.ToString("N0", culture));
                            table.Cell().AlignRight().Text(p.Total.ToString("N0", culture));

                            table.Cell().Text("");
                            table.Cell().Text("");
                            table.Cell().Text("");
                            table.Cell().Text("");
                        }
                    });

                    // Tổng
                    column.Item().LineHorizontal(1);
                    column.Item().Text($"Ghi chú : {Note}");
                    column.Item().Text($"Số lượng : {TotalQuantity}");
                    column.Item().Text($"Cộng tiền hàng : {TotalAmount.ToString("N0", culture)}");
                    column.Item().Text($"Khách phải trả : {TotalAmount.ToString("N0", culture)}").Bold();

                });

                page.Footer().AlignCenter().Text("Cảm ơn quý khách!").FontSize(10);
            });
        }
    }
}
