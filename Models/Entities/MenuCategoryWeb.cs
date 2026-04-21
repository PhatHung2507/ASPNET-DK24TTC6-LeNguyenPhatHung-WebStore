using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class MenuCategoryWeb
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IconClass { get; set; }
        public int SortOrder { get; set; }
        public List<MenuItemWeb> MenuItemWeb { get; set; } = new();
    }
}
