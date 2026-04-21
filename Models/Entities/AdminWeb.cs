using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class AdminWeb : BaseTable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; }
        public bool AdminYn { get; set; }
        public string? PermissionId { get; set; }
        public Permission? Permission { get; set; }
    }
}
