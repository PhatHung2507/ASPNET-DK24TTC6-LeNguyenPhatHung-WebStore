using WebStore.Models.Entities;

namespace WebStore.Models
{
    public class PermissionMenuViewModel
    {
        public string CategoryName { get; set; }
        public List<MenuItemViewModel> Items { get; set; }
    }
}
