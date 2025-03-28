
using EShop.Domain.Identity;

namespace  EShop.Domain.DomainModels
{ 
    public class Order : BaseEntity
    {
        public string UserOrderId { get; set; }
        public EShopApplicationUser User { get; set; }

        public virtual ICollection<ProductInOrder> Products { get; set; }
    }
}
