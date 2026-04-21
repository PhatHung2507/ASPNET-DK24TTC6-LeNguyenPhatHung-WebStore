using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class Permission : BaseTable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<PermissionMenu> PermissionMenu { get; set; } = new();
        public List<AdminWeb> AdminWeb { get; set; } = new();
    }
}
