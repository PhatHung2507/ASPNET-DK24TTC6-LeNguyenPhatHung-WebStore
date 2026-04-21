using Microsoft.AspNetCore.Http;
using System;

public static class CookieHelper
{
    public static void SetCookie(HttpResponse response, string key, string value, int? expireDays = null)
    {
        var options = new CookieOptions();
        options.Expires = expireDays.HasValue ? DateTime.Now.AddDays(expireDays.Value) : DateTime.Now.AddHours(1);
        response.Cookies.Append(key, value, options);
    }

    public static string GetCookie(HttpRequest request, string key)
    {
        return request.Cookies[key];
    }
}
