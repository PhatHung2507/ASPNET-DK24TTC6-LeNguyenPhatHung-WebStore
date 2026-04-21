using Microsoft.AspNetCore.Mvc;

namespace WebStore.Models
{
    public class PageViewModel<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public string? BaseUrl { get; set; } 
        public string? ReloadFunctionName { get; set; } 
    }
    public class PaginationFilter
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
