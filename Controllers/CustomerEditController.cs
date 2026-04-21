using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("list/customeredit")]
    public class CustomerEditController : BaseController
    {
        private readonly AppDbContext _context;
        public CustomerEditController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? id)
        {
            var viewModel = new CustomerAddressViewModel
            {
                Locations = await _context.location.ToListAsync(),
                Wards = await _context.ward.ToListAsync()
            };

            if (!string.IsNullOrEmpty(id))
            {
                var item = await _context.customer.FindAsync(id);
                if (item == null) return NotFound("Không tìm thấy");
                viewModel.Customer = item;
            }
            return PartialView("~/Views/List/CustomerEdit.cshtml", viewModel);
        }
        [HttpGet("GetWardsByLocation")]
        public JsonResult GetWardsByLocation(string locationId)
        {
            var wards = _context.ward
                .Where(w => w.LocationId == locationId)
                .Select(w => new { id = w.Id, name = w.Name })
                .ToList();

            return Json(wards);
        }
        [HttpPost("")]
        public async Task<IActionResult> Index(Customer model)
        {
            string CurrentAdmin = HttpContext.Session.GetString("UserId") ??"";
            if (string.IsNullOrEmpty(model.Id))
            {
                // Trường hợp thêm mới, model.Id chưa có
                model.Id = Guid.NewGuid().ToString();
                model.UserCreate = CurrentAdmin;
                model.DateCreate = DateTime.Now;
                model.UserUpdate = CurrentAdmin;
                model.DateUpdate = DateTime.Now;
                _context.customer.Add(model);
            }
            else
            {
                // Trường hợp cập nhật: lấy bản ghi gốc theo Id từ model.Id
                var existing = await _context.customer.FindAsync(model.Id);
                if (existing == null)
                {
                    return NotFound("Không tìm thấy.");
                }
                // Cập nhật các trường ngoại trừ Id
                existing.Name = model.Name;
                existing.Tel = model.Tel;
                existing.Gender = model.Gender;
                existing.BirthDay = model.BirthDay;
                existing.Email = model.Email;
                existing.UserUpdate = CurrentAdmin;
                existing.DateUpdate = DateTime.Now;
                existing.LocationId = model.LocationId;
                existing.WardId = model.WardId;
                existing.Address = model.Address;
                existing.Note = model.Note;
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu thành công" });
        }
    }
}
