using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShop.Domain.DomainModels;
using EShop.Domain.DTO;

namespace EShop.Service.Interface
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product GetDetailsForProduct(Guid? id);
        void CreateNewProduct(Product product);
        void UpdateExistingProduct(Product p);
        AddToShoppingCardDto GetShoppingCardInfo(Guid? id);
        void DeleteProduct(Guid? id);
        bool AddToShoppingCart(AddToShoppingCardDto item,string userID);


    }
}
