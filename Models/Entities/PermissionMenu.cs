using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class PermissionMenu
    {
        public string Id { get; set; }
        public string PermissionId { get; set; }
        public string MenuItemWebId { get; set; }
        public bool? See { get; set; }
        public bool? New { get; set; }
        public bool? Edit { get; set; }
        public bool? Remove { get; set; }
        public Permission Permission { get; set; }
        public MenuItemWeb MenuItemWeb { get; set; }
    }
}
