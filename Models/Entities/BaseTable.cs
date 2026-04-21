using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Entities
{
    public class BaseTable
    {
        public bool? Active { get; set; }
        public string? UserCreate { get; set; }
        public DateTime? DateCreate { get; set; }
        public string? UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
