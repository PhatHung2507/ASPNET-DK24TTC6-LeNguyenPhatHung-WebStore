using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Models;
using WebStore.Models.Entities;

namespace WebStore.ViewComponents
{
    public class PermissionButtonsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionButtonsViewComponent(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string menuId,string modal)
        {
            var menuItem = await _context.menuItemWeb.FirstOrDefaultAsync(x => x.Id == menuId);

            string _name = menuItem.Name;
            string _listUrl = menuItem.Url;
            string _editUrl = menuItem.EditUrl;
            bool _CanCreate = menuItem?.New.GetValueOrDefault() ?? false;
            bool _CanEdit = menuItem?.Edit.GetValueOrDefault() ?? false;
            bool _CanDelete = menuItem?.Remove.GetValueOrDefault() ?? false;

            var isAdmin = _httpContextAccessor.HttpContext!.Session!.GetString("Role") == "True";
            if (isAdmin)
            {
                return View("Default", new PermissionActionViewModel { CanCreate = _CanCreate, CanEdit = _CanEdit, CanDelete = _CanDelete, Name = _name,ListUrl = _listUrl,EditUrl = _editUrl, Modal = modal });
            }

            var permissionId = _httpContextAccessor.HttpContext.Session.GetString("PermissionId");
            if (string.IsNullOrEmpty(permissionId)) return View("Default", new PermissionActionViewModel());

            var permissionMenu = await _context.permissionMenu
                .FirstOrDefaultAsync(x => x.PermissionId == permissionId && x.MenuItemWebId == menuId);

            if (permissionMenu == null) return View("Default", new PermissionActionViewModel());

            return View("Default", new PermissionActionViewModel
            {
                CanCreate = permissionMenu.New ?? false,
                CanEdit = permissionMenu.Edit ?? false,
                CanDelete = permissionMenu.Remove ?? false,
                Name = _name,
                ListUrl = _listUrl,
                EditUrl = _editUrl,
                Modal = modal,
            });
        }
    }
    public class PermissionActionViewModel
    {
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public string Name;
        public string ListUrl { get; set; }
        public string EditUrl { get; set; }
        public string Modal { get; set; }
    }
}
