using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("sale/Ordersedit")]
    public class OrdersEditController : BaseController
    {
        private readonly AppDbContext _context;
        public OrdersEditController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? id)
        {
            var viewModel = new OrdersMasterViewModel
            {
                Customers = await _context.customer.ToListAsync(),
                Products = await _context.product.ToListAsync(),
                OrdersDetails = await _context.ordersDetail.Where(x=>x.OrdersMasterId == id).ToListAsync()
            };

            if (!string.IsNullOrEmpty(id))
            {
                var item = await _context.ordersMaster.FindAsync(id);
                if (item == null) return NotFound("Không tìm thấy menu.");
                viewModel.OrdersMaster = item;
            }
            else
            {
                viewModel.OrdersMaster = new OrdersMaster
                {
                    VoucherDate = DateTime.Now.Date
                };
            }    
            return PartialView("~/Views/Sale/OrdersEdit.cshtml", viewModel);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(OrdersMaster model)
        {
            string CurrentAdmin = HttpContext.Session.GetString("UserId") ??"";
            if (string.IsNullOrEmpty(model.Id))
            {
                // Trường hợp thêm mới, model.Id chưa có
                model.Id = Guid.NewGuid().ToString();
                model.Status = "Chưa thanh toán";
                model.UserCreate = CurrentAdmin;
                model.DateCreate = DateTime.Now;
                model.UserUpdate = CurrentAdmin;
                model.DateUpdate = DateTime.Now;
                _context.ordersMaster.Add(model);
            }
            else
            {
                // Trường hợp cập nhật: lấy bản ghi gốc theo Id từ model.Id
                var existing = await _context.ordersMaster.FindAsync(model.Id);
                if (existing == null)
                {
                    return NotFound("Không tìm thấy.");
                }
                // Cập nhật các trường ngoại trừ Id
                existing.VoucherDate = model.VoucherDate;
                existing.CustomerId = model.CustomerId;
                existing.TotalMoney = model.TotalMoney;
                existing.TotalQuantity = model.TotalQuantity;
                existing.Source = model.Source;
                existing.UserUpdate = CurrentAdmin;
                existing.DateUpdate = DateTime.Now;
                existing.DiscountPercent = model.DiscountPercent;
                existing.DiscountMoney = model.DiscountMoney;
                existing.TotalDiscount = model.TotalDiscount;
                existing.IsDiscountPercent = model.IsDiscountPercent;
                existing.IsDiscountMoney = model.IsDiscountMoney;
                existing.MoneyShip = model.MoneyShip;
                existing.NeedPayment = model.NeedPayment;
                existing.CustomerPayment = model.CustomerPayment;
                existing.Note = model.Note;
                if(model.NeedPayment <= model.CustomerPayment)
                {
                    existing.Status = "Đã thanh toán";
                }    
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu thành công" });
        }
        [HttpPost]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct(string productId, int quantity,string idMaster,DateTime voucherDate,string cusId, decimal priceInput)
        {
            var quantityIn = _context.importStockDetail.Where(x => x.ProductId == productId).Sum(x=>x.Quantity ?? 0);
            var quantityOut = _context.ordersDetail.Where(x => x.ProductId == productId).Sum(x => x.Quantity ?? 0);
            if((quantityIn - quantityOut) < quantity)
            {
                return StatusCode(409, "Số lượng tồn không đủ");
            }    
            if (string.IsNullOrEmpty(idMaster))
            {
                string CurrentAdmin = HttpContext.Session.GetString("UserId") ?? "";
                idMaster = Guid.NewGuid().ToString();
                var ordersMaster = new OrdersMaster();
                ordersMaster.Id = idMaster;
                ordersMaster.VoucherDate = voucherDate;
                if(!string.IsNullOrEmpty(cusId))
                {
                    ordersMaster.CustomerId = cusId;
                }
                ordersMaster.TotalMoney = 0;
                ordersMaster.TotalQuantity = 0;
                ordersMaster.UserCreate = CurrentAdmin;
                ordersMaster.DateCreate = DateTime.Now;
                ordersMaster.UserUpdate = CurrentAdmin;
                ordersMaster.DateUpdate = DateTime.Now;
                ordersMaster.Status = "Chưa thanh toán";
                _context.ordersMaster.Add(ordersMaster);
                await _context.SaveChangesAsync();
            }
            var product = _context.product.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return BadRequest();
            }

            var price = priceInput;
            var money = quantity * price;

            var detail = new OrdersDetail();
            detail.Id = Guid.NewGuid().ToString();
            detail.OrdersMasterId = idMaster;
            detail.ProductId = productId;
            detail.Quantity = quantity;
            detail.Price = price;
            detail.TotalMoney = money;
            _context.ordersDetail.Add(detail);
            await _context.SaveChangesAsync();

            decimal totalQuantity = _context.ordersDetail
            .Where(x => x.OrdersMasterId == idMaster)
            .Sum(x => x.Quantity ?? 0);

            decimal totalMoney = _context.ordersDetail
            .Where(x => x.OrdersMasterId == idMaster)
            .Sum(x => x.TotalMoney ?? 0);

            var master = _context.ordersMaster.FirstOrDefault(p => p.Id == idMaster);
            master.TotalMoney = totalMoney;
            master.TotalQuantity = totalQuantity;
            await _context.SaveChangesAsync();

            // Trả về dữ liệu JSON
            return Json(new
            {
                productName = product.Name,
                quantity = quantity.ToString("N0"),
                price = price.ToString("N0"),
                money = money.ToString("N0"),
                iddetail = detail.Id,
                idOrdersMasterId = detail.OrdersMasterId,
                totalMoney = totalMoney
            });
        }
        [HttpGet]
        [Route("LoadPriceProduct")]
        public async Task<IActionResult> LoadPriceProduct(string id)
        {
            decimal price = 0;
            var product = _context.product.FirstOrDefault(p => p.Id == id);
            if (product != null && product.SalePrice != null)
            {
                price = product.SalePrice.Value;
            }
            return Json(new
            {
                price = price
            });
        }
        [HttpPost]
        [Route("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var item = _context.ordersDetail.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return BadRequest();
            }

            _context.ordersDetail.Remove(item);
            _context.SaveChanges();

            decimal totalQuantity = _context.ordersDetail
            .Where(x => x.OrdersMasterId == item.OrdersMasterId)
            .Sum(x => x.Quantity ?? 0);

            decimal totalMoney = _context.ordersDetail
            .Where(x => x.OrdersMasterId == item.OrdersMasterId)
            .Sum(x => x.TotalMoney ?? 0);

            var master = _context.ordersMaster.FirstOrDefault(p => p.Id == item.OrdersMasterId);
            master.TotalMoney = totalMoney;
            master.TotalQuantity = totalQuantity;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
    }

}
