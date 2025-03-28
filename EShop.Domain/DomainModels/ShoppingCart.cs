
using EShop.Domain.Identity;

namespace EShop.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
      
        public String OwnerId { get; set; }
        public EShopApplicationUser Owner { get; set; }
        public virtual ICollection<ProductInShoppingCart> ProductinShoppingCart { get; set; }

        public ShoppingCart() { }
    }
}
