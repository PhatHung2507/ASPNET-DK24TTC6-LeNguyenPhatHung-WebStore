using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;

namespace WebStore.Controllers
{
    public class ProfileStoreController : BaseStoreController
    {
        private readonly AppDbContext _context;
        public ProfileStoreController(AppDbContext context)
        {
            _context = context;
        }
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
            return PartialView("~/Views/User/ProfileStore/ProfileStore.cshtml", viewModel);
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
        [HttpPost]
        public async Task<IActionResult> Index(Customer model)
        {
            string CurrentAdmin = HttpContext.Session.GetString("UserId") ?? "";
            if (!string.IsNullOrEmpty(model.Id))
            {
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
                existing.Password = model.Password;
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật thành công!";
            return RedirectToAction("Index", new { id = model.Id });
        }
    }
}
