using WebStore.Models.Entities;

namespace WebStore.Models
{
    public class AdminWebByPermissionViewModel
    {
        public AdminWeb AdminWeb { get; set; } = new AdminWeb();
        public List<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
