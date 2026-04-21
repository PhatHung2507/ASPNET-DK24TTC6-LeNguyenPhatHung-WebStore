using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Diagnostics;
using WebStore.Models;
using WebStore.Models.Entities;

namespace WebStore.Controllers
{
    public class BaseStoreController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cartJson = Request.Cookies["Cart"];
            List<CartItem> cart = string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            ViewBag.CartQuantity = cart.Sum(x => x.Quantity);

            base.OnActionExecuting(context);
        }
    }
}
