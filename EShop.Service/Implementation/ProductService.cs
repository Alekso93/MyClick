using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Service.Interface;
using EShop.Repository.Interface;
using Microsoft.Extensions.Logging;

namespace EShop.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<HostExecutionContext> _logger;

        public ProductService(IUserRepository userRepository, IRepository<ProductInShoppingCart> productInShoppingCartRepository, IRepository<Product> productRepository, ILogger<HostExecutionContext> logger)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
            _logger = logger;
        }

        public bool AddToShoppingCart(AddToShoppingCardDto item, string userID)
        {
            if (string.IsNullOrEmpty(userID))
            {
                throw new ArgumentNullException(nameof(userID), "User ID cannot be null or empty.");
            }

            var user = this._userRepository.Get(userID);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (user.UserCart == null)
            {
                user.UserCart = new ShoppingCart
                {
                    OwnerId = user.Id,
                    Owner = user,
                    ProductinShoppingCart = new List<ProductInShoppingCart>()
                };
                this._userRepository.Update(user);
            }

            var userShoppingCard = user.UserCart;
            if (userShoppingCard != null && item.ProductId != null)
            {
                var product = this.GetDetailsForProduct(item.ProductId);
                if (product != null)
                {
                    ProductInShoppingCart itemToAdd = new ProductInShoppingCart
                    {
                        Id= Guid.NewGuid(),
                        Product = product,
                        ProductId = product.Id,
                        ShoppingCart = userShoppingCard,
                        ShoppingCartId = userShoppingCard.Id,
                        Quantity = item.Quantity
                    };
                    this._productInShoppingCartRepository.Insert(itemToAdd);
                    _logger.LogInformation("Product added to shopping cart.");
                    return true;
                }
                return false;
            }
            _logger.LogInformation("Something was wrong,check ProductId or UserShoppingCart");
            return false;
        }

        public void CreateNewProduct(Product p)
        {
           this._productRepository.Insert(p);
        }

        public void DeleteProduct(Guid? id)
        {
            var product=this.GetDetailsForProduct(id);
            this._productRepository.Delete(product);
        }

        public List<Product> GetAllProducts()
        {
            _logger.LogInformation("GetAllProducts method called.");
            return this._productRepository.GetAll().ToList();
        }

        public Product GetDetailsForProduct(Guid? id)
        {
            return this._productRepository.Get(id);
        }

        public AddToShoppingCardDto GetShoppingCardInfo(Guid? id)
        {
            var product=this.GetDetailsForProduct(id);
            AddToShoppingCardDto model = new AddToShoppingCardDto
            {
                SelectedProduct = product,
                ProductId = product.Id,
                Quantity = 1
            };
            return model;
        }

        public void UpdateExistingProduct(Product p)
        {
            this._productRepository.Update(p);
        }
    }
}
