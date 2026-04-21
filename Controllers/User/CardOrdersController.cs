using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Controllers;
using WebStore.Models;
using WebStore.Models.Entities;

public class CardOrdersController : BaseStoreController
{
    private readonly AppDbContext _context;

    public CardOrdersController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy giỏ hàng từ cookie
        string cartJson = Request.Cookies["Cart"];
        List<CartItem> cart = string.IsNullOrEmpty(cartJson)
            ? new List<CartItem>()
            : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

        decimal total = cart.Sum(item => item.Price * item.Quantity);
        // ✅ THÊM ĐOẠN NÀY NGAY ĐÂY
        var totalAll = total + 30000;
        string qrUrl = $"https://img.vietqr.io/image/970422-1234567890-compact.png?amount={totalAll}&addInfo=ThanhToanDonHang";
        ViewBag.QR = qrUrl;
        var customerId = HttpContext.Session.GetString("UserCustomerId");
        Customer cus = new Customer();
        if(!string.IsNullOrEmpty(customerId))
        {
            cus = await _context.customer
            .Include(c => c.Location)
            .Include(c => c.Ward)
            .FirstOrDefaultAsync(c => c.Id == customerId);
        }    
        var vm = new CartViewModel
        {
            Items = cart,
            TotalTemp = total,
            TotalAll = total + 30000,
            Customer = cus
        };

        return View("~/Views/User/CardOrders/CardOrders.cshtml", vm);
    }
    [HttpPost]
    public JsonResult UpdateQuantity(string productId, int quantity)
    {
        string cartJson = Request.Cookies["Cart"];
        List<CartItem> cart = string.IsNullOrEmpty(cartJson)
            ? new List<CartItem>()
            : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity = quantity;
        }

        string updated = JsonConvert.SerializeObject(cart);
        Response.Cookies.Append("Cart", updated, new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(7)
        });

        return Json(new { success = true });
    }
    [HttpPost]
    public JsonResult RemoveFromCart(string productId)
    {
        string cartJson = Request.Cookies["Cart"];
        List<CartItem> cart = string.IsNullOrEmpty(cartJson)
            ? new List<CartItem>()
            : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Remove(item);
        }

        string updated = JsonConvert.SerializeObject(cart);
        Response.Cookies.Append("Cart", updated, new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(7)
        });

        return Json(new { success = true });
    }
    [HttpPost]
    public async Task<JsonResult> PlaceOrder()
    {
        var customerId = HttpContext.Session.GetString("UserCustomerId");

        if (string.IsNullOrEmpty(customerId))
        {
            return Json(new { success = false, message = "Bạn cần đăng nhập để đặt hàng." });
        }

        var customer = await _context.customer.FindAsync(customerId);
        if (customer == null)
        {
            return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng." });
        }

        string cartJson = Request.Cookies["Cart"];
        List<CartItem> cart = string.IsNullOrEmpty(cartJson)
            ? new List<CartItem>()
            : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

        if (cart == null || !cart.Any())
        {
            return Json(new { success = false, message = "Giỏ hàng của bạn đang trống." });
        }

        var totalMoney = cart.Sum(i => i.Price * i.Quantity);
        var totalQuantity = cart.Sum(i => i.Quantity);

        var order = new OrdersMaster
        {
            Id = Guid.NewGuid().ToString(),
            VoucherDate = DateTime.Now,
            CustomerId = customerId,
            TotalMoney = totalMoney,
            TotalQuantity = totalQuantity,
            Status = "Chưa thanh toán", // trạng thái đơn hàng
            MoneyShip = 30000,
            NeedPayment = totalMoney + 30000,
            Source = "Website",
            OrdersDetail = new List<OrdersDetail>()
        };

        foreach (var item in cart)
        {
            var detail = new OrdersDetail
            {
                Id = Guid.NewGuid().ToString(),
                OrdersMasterId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price,
                TotalMoney = item.Price * item.Quantity,
                Active = true
            };
            order.OrdersDetail.Add(detail);
        }

        _context.ordersMaster.Add(order);
        await _context.SaveChangesAsync();

        // Xóa cookie giỏ hàng sau khi đặt hàng
        Response.Cookies.Delete("Cart");

        return Json(new { success = true });
    }
}
