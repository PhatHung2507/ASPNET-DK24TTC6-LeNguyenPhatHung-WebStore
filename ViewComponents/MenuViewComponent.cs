using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Models;
using WebStore.Models.Entities;

namespace WebStore.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuViewComponent(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var isAdmin = _httpContextAccessor.HttpContext.Session.GetString("Role") == "True";
            var permissionId = _httpContextAccessor.HttpContext.Session.GetString("PermissionId");

            List<MenuCategoryWeb> result;

            if (isAdmin)
            {
                result = await _context.menuCategoryWeb
                    .Include(c => c.MenuItemWeb.Where(x=>x.Visible == true))
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync();
            }
            else
            {
                var allowedMenuItemIds = await _context.permissionMenu
                    .Where(pm => pm.PermissionId == permissionId && pm.See == true)
                    .Select(pm => pm.MenuItemWebId)
                    .ToListAsync();

                result = await _context.menuCategoryWeb
                    .Include(c => c.MenuItemWeb.Where(i => allowedMenuItemIds.Contains(i.Id)))
                    .Where(c => c.MenuItemWeb.Any(i => allowedMenuItemIds.Contains(i.Id)))
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync();
            }

            return View(result);
        }
    }

}
