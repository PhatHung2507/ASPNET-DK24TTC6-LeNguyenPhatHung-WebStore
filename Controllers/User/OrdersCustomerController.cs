using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Controllers;
using WebStore.Models;
using WebStore.Models.Entities;

public class OrdersCustomerController : BaseStoreController
{
    private readonly AppDbContext _context;

    public OrdersCustomerController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var customerId = HttpContext.Session.GetString("UserCustomerId");

        if (string.IsNullOrEmpty(customerId))
        {
            return RedirectToAction("HomeStore", "Index"); 
        }

        var orders = await _context.ordersMaster
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.OrdersDetail)
                .ThenInclude(od => od.Product)
            .OrderByDescending(o => o.VoucherDate)
            .ToListAsync();

        return View("~/Views/User/OrdersCustomer/OrdersCustomer.cshtml", orders);
    }
}
