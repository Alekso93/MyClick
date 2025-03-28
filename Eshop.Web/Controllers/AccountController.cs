using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Eshop.Web.Data;
using EShop.Domain.Identity;
using NuGet.Protocol.Plugins;
using System.Threading.Tasks;
using EShop.Domain.DomainModels;

namespace Eshop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<EShopApplicationUser> userManager;
        private readonly SignInManager<EShopApplicationUser> signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<EShopApplicationUser> userManager, SignInManager<EShopApplicationUser> signInManager,ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = context;
        }

        public IActionResult Register()
        {
            UserRegistrationDto model = new UserRegistrationDto();
            return View(model);
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new EShopApplicationUser
                    {
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        Address = "",
                        UserCart = new ShoppingCart(),

                    };
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        var shoppingCart = new ShoppingCart
                        {
                            OwnerId = user.Id,
                            Owner = user
                        };
              

                        if (shoppingCart.OwnerId == null)
                        {
                            ModelState.AddModelError("message", "OwnerId is null");
                            return View(request);
                        }
                        Console.WriteLine($"ShoppingCart OwnerId: {shoppingCart.OwnerId}, ShoppingCartId: {shoppingCart.Id}"); 

                        // Додади ја картичката во базата
                        _context.ShoppingCarts.Add(shoppingCart);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists");
                    return View(request);
                }
            }
            return View(request);
        }

    }
}
