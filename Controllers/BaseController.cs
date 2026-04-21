using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class BaseController : Controller
    {
        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            var userId = httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                httpContextAccessor.HttpContext.Response.Redirect("/");
            }
        }
    }
}
