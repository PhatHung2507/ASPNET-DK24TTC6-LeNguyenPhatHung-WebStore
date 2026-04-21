using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;
using WebStore.Service;

namespace WebStore.Controllers
{
    [Route("sale/ImportStockedit")]
    public class ImportStockEditController : BaseController
    {
        private readonly AppDbContext _context;
        public ImportStockEditController(AppDbContext context, IHttpContextAccessor accessor)
        : base(accessor)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? id)
        {
            var viewModel = new ImportStockMasterViewModel
            {
                Suppliers = await _context.supplier.ToListAsync(),
                Products = await _context.product.ToListAsync(),
                ImportStockDetails = await _context.importStockDetail.Where(x=>x.ImportStockMasterId == id).ToListAsync()
            };

            if (!string.IsNullOrEmpty(id))
            {
                var item = await _context.importStockMaster.FindAsync(id);
                if (item == null) return NotFound("Không tìm thấy menu.");
                viewModel.ImportStockMaster = item;
            }
            else
            {
                viewModel.ImportStockMaster = new ImportStockMaster
                {
                    VoucherDate = DateTime.Now.Date
                };
            }    
            return PartialView("~/Views/Sale/ImportStockEdit.cshtml", viewModel);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(ImportStockMaster model)
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
                _context.importStockMaster.Add(model);
            }
            else
            {
                // Trường hợp cập nhật: lấy bản ghi gốc theo Id từ model.Id
                var existing = await _context.importStockMaster.FindAsync(model.Id);
                if (existing == null)
                {
                    return NotFound("Không tìm thấy.");
                }
                // Cập nhật các trường ngoại trừ Id
                existing.VoucherDate = model.VoucherDate;
                existing.SupplierId = model.SupplierId;
                existing.UserUpdate = CurrentAdmin;
                existing.DateUpdate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu thành công" });
        }
        [HttpPost]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct(string productId, int quantity,string idMaster,DateTime voucherDate,string supId,decimal priceInput)
        {
            if(string.IsNullOrEmpty(idMaster))
            {
                string CurrentAdmin = HttpContext.Session.GetString("UserId") ?? "";
                idMaster = Guid.NewGuid().ToString();
                var stockMaster = new ImportStockMaster();
                stockMaster.Id = idMaster;
                stockMaster.VoucherDate = voucherDate;
                if(!string.IsNullOrEmpty(supId))
                {
                    stockMaster.SupplierId = supId;
                }    
                stockMaster.TotalMoney = 0;
                stockMaster.TotalQuantity = 0;
                stockMaster.UserCreate = CurrentAdmin;
                stockMaster.DateCreate = DateTime.Now;
                stockMaster.UserUpdate = CurrentAdmin;
                stockMaster.DateUpdate = DateTime.Now;
                _context.importStockMaster.Add(stockMaster);
                await _context.SaveChangesAsync();
            }
            var product = _context.product.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return BadRequest();
            }

            var price = priceInput;
            var money = quantity * price;

            var detail = new ImportStockDetail();
            detail.Id = Guid.NewGuid().ToString();
            detail.ImportStockMasterId = idMaster;
            detail.ProductId = productId;
            detail.Quantity = quantity;
            detail.Price = price;
            detail.TotalMoney = money;
            _context.importStockDetail.Add(detail);
            await _context.SaveChangesAsync();

            decimal totalQuantity = _context.importStockDetail
            .Where(x => x.ImportStockMasterId == idMaster)
            .Sum(x => x.Quantity ?? 0);

            decimal totalMoney = _context.importStockDetail
            .Where(x => x.ImportStockMasterId == idMaster)
            .Sum(x => x.TotalMoney ?? 0);

            var master = _context.importStockMaster.FirstOrDefault(p => p.Id == idMaster);
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
                idImportStockMasterId = detail.ImportStockMasterId
            });
        }
        [HttpGet]
        [Route("LoadPriceProduct")]
        public async Task<IActionResult> LoadPriceProduct(string id)
        {
            decimal price = 0;
            var product = _context.product.FirstOrDefault(p => p.Id == id);
            if (product != null && product.PurchasePrice != null)
            {
                price = product.PurchasePrice.Value;
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
            var item = _context.importStockDetail.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return BadRequest();
            }

            _context.importStockDetail.Remove(item);
            _context.SaveChanges();

            decimal totalQuantity = _context.importStockDetail
            .Where(x => x.ImportStockMasterId == item.ImportStockMasterId)
            .Sum(x => x.Quantity ?? 0);

            decimal totalMoney = _context.importStockDetail
            .Where(x => x.ImportStockMasterId == item.ImportStockMasterId)
            .Sum(x => x.TotalMoney ?? 0);

            var master = _context.importStockMaster.FirstOrDefault(p => p.Id == item.ImportStockMasterId);
            master.TotalMoney = totalMoney;
            master.TotalQuantity = totalQuantity;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
    }

}
