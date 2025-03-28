using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShop.Domain.DTO;

namespace EShop.Service.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetShoppingCartInfo(string userId);
        bool deleteProductFromShoppingCart(string UserId,Guid id);
        bool OrderNow(string userId);
    }
}
