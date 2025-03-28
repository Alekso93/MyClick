using EShop.Domain.DomainModels;
using Microsoft.AspNetCore.Identity;

namespace EShop.Domain.Identity

{
    public class EShopApplicationUser : IdentityUser
    {
        public String? FirstName { get; set; }
        public String? LastName { get; set; }
        public String? Address { get; set; }
        public virtual ShoppingCart UserCart { get; set; }
    }
}
