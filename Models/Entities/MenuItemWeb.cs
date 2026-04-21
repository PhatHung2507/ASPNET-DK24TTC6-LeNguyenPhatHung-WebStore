using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class MenuItemWeb
    {
        public string Id { get; set; }
        public string MenuCategoryWebId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string? EditUrl { get; set; }
        public int SortOrder { get; set; }
        public bool? See { get; set; }
        public bool? New { get; set; }
        public bool? Edit { get; set; }
        public bool? Remove { get; set; }
        public MenuCategoryWeb MenuCategoryWeb { get; set; }
        public List<PermissionMenu> PermissionMenu { get; set; } = new();
        public Boolean Visible { get; set; }
    }
    public class MenuItemViewModel
    {
        public string Id { get; set; }         // MenuItemWebId
        public string Name { get; set; }
        public bool HasSee { get; set; }        // Từ MenuItemWeb.New
        public bool HasNew { get; set; }
        public bool HasEdit { get; set; }
        public bool HasRemove { get; set; }

        public bool IsSeeChecked { get; set; } // Từ PermissionMenu
        public bool IsNewChecked { get; set; }
        public bool IsEditChecked { get; set; }
        public bool IsRemoveChecked { get; set; }
    }
    public class MenuItemByCategoryViewModel
    {
        public MenuItemWeb MenuItemWeb { get; set; } = new MenuItemWeb();
        public List<MenuCategoryWeb> MenuCategories { get; set; } = new List<MenuCategoryWeb>();
    }
}
