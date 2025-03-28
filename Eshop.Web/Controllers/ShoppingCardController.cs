using System.Security.Claims;
using Eshop.Web.Data;
using EShop.Domain;
using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Domain.Identity;
using EShop.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Eshop.Web.Controllers
{
    public class ShoppingCardController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCardController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           

            // var allProducts = userShoppingCart.ProductinShoppingCart.Select(z => z.Product).ToList();
            return View(this._shoppingCartService.GetShoppingCartInfo(userId));
        }
        public IActionResult OrderSuccess()
        {
            ViewData["Message"] = "Your payment was successful!";
            return View();
        }

        public IActionResult OrderFailed()
        {
            ViewData["Message"] = "Payment failed. Please try again.";
            return View();
        }

       
        [HttpPost]
        public IActionResult PayOrder(string stripeEmail,string stripeToken)
        {
           var customerService = new CustomerService();
            var chargeService = new ChargeService();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shoppingCart = this._shoppingCartService.GetShoppingCartInfo(userId);
            
            var stripeCustomer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });
            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (long)shoppingCart.TotalPrice * 100,
                Description = "EShop Purchase",
                Currency = "usd",
                Customer = stripeCustomer.Id
            });
            if (charge.Status=="succeeded")
            {
                var result = this.OrderNow();
                if (result)
                {
                    return RedirectToAction("OrderSuccess", "ShoppingCard");
                }
                else
                {
                    return RedirectToAction("OrderFailed", "ShoppingCard");
                }
            }
            else
            {
                return RedirectToAction("OrderFailed", "ShoppingCard");
            }
        }


        public IActionResult DeleteProductFromShoppingCart(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result=this._shoppingCartService.deleteProductFromShoppingCart(userId, productId);
            if(result)
            {
                return RedirectToAction("Index", "ShoppingCard");
            }
            else
            {
                return RedirectToAction("Index", "ShoppingCard");
            }
        }
        private Boolean OrderNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.OrderNow(userId);

            return result;

        }
    }
}
