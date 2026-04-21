using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using WebStore.Models;

namespace WebStore.Service
{
    public static class Helpers
    {
        public static async Task<PageViewModel<T>> PaginateAsync<T>(IQueryable<T> query, int page,int pageSize)
        {
            var totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PageViewModel<T>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
        public static Dictionary<string, string> ExtractFilters(IQueryCollection query)
        {
            return query
                .Where(q => !string.Equals(q.Key, "page", StringComparison.OrdinalIgnoreCase)
                         && !string.Equals(q.Key, "pageSize", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(q => q.Key, q => q.Value.ToString());
        }
        public static string ConvertNameToCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            input = input.Replace('Đ', 'D').Replace('đ', 'd');
            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            string noAccent = sb.ToString().Normalize(NormalizationForm.FormC);
            string noSpaces = System.Text.RegularExpressions.Regex.Replace(noAccent, @"\s+", "");
            return noSpaces.ToUpperInvariant();
        }
        public static string ToMd5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); 
                }
                return sb.ToString();
            }
        }
    }
}
