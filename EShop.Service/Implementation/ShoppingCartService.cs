using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Repository.Interface;
using EShop.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace EShop.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _OrderRepository;
        private readonly IRepository<ProductInOrder> _ProductInOrderRepository;
        private readonly IRepository<EmailMessage> _mailRepository;

        private readonly IUserRepository _userRepository;

        public ShoppingCartService(IRepository<EmailMessage> mailRepository,IRepository<ShoppingCart> shoppingCartRepository,IUserRepository userRepository,IRepository<Order> OrderRepository, IRepository<ProductInOrder> ProductInOrderRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _OrderRepository = OrderRepository;
            _ProductInOrderRepository = ProductInOrderRepository;
            _mailRepository = mailRepository;
        }

        public ShoppingCartDto GetShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);
            if (loggedInUser.UserCart == null)
            {
                loggedInUser.UserCart = new ShoppingCart
                {
                    OwnerId = loggedInUser.Id,
                    Owner = loggedInUser,
                    ProductinShoppingCart = new List<ProductInShoppingCart>()
                };
                this._userRepository.Update(loggedInUser);
            }
            var userShoppingCart = loggedInUser.UserCart;
            var allProducts = userShoppingCart.ProductinShoppingCart.ToList();
            var allProductsPrice = allProducts.Select(z => new
            {
                ProductPrice = z.Product.ProductPrice,
                Quantity = z.Quantity
            }).ToList();

            var totalPrice = 0;
            foreach (var item in allProductsPrice)
            {
                totalPrice += item.ProductPrice * item.Quantity;
            }
            ShoppingCartDto ShoppingCartDtoitem = new ShoppingCartDto
            {
                Products = allProducts,
                TotalPrice = totalPrice
            };
            return ShoppingCartDtoitem;
        }
        public bool deleteProductFromShoppingCart(string UserId, Guid id)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return false;
            }

            // Вклучи ги релациите (EF Core)
            var loggedInUser = _userRepository.Get(UserId);
                                

            if (loggedInUser?.UserCart == null)
            {
                return false;
            }

            var userShoppingCard = loggedInUser.UserCart;

            // Барај го производот со даденото ID
            var itemToDelete = userShoppingCard.ProductinShoppingCart
                                  .FirstOrDefault(z => z.ProductId == id);

            if (itemToDelete != null)
            {
                userShoppingCard.ProductinShoppingCart.Remove(itemToDelete);
                _shoppingCartRepository.Update(userShoppingCard);
                _shoppingCartRepository.SaveChanges(); // Зачувај ги промените
                return true;
            }

            return false;
        }

        public bool OrderNow(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var loggedInUser = this._userRepository.Get(userId);

            if (loggedInUser?.UserCart == null)
            {
                return false;
            }

            var UserShoppingCart = loggedInUser.UserCart;
            EmailMessage message= new EmailMessage();
            message.MailTo = loggedInUser.Email;
            message.Subject = "Order Confirmation Andrej";
            message.Status = false;

            Order orderItem = new Order
            {
                Id = Guid.NewGuid(),
                UserOrderId = userId,
                User = loggedInUser
            };

            this._OrderRepository.Insert(orderItem);

            List<ProductInOrder> productInOrder = new List<ProductInOrder>();

            var result = UserShoppingCart.ProductinShoppingCart.Select
                (z => new ProductInOrder
                {
                    OrderId = orderItem.Id,
                    ProductId = z.Product.Id,
                    SelectedProduct = z.Product,
                    UserOrder = orderItem,
                    Quantity= z.Quantity
                }).ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Your order is confirmed. The products you ordered are: ");
            var TotalPrice = 0.0;
            for (int i = 0; i < result.Count; i++)
            {
                var item = result[i];
                TotalPrice += item.SelectedProduct.ProductPrice * item.Quantity;
                sb.AppendLine((i + 1).ToString() + ". " + item.SelectedProduct.ProductName + " with price of: " + item.SelectedProduct.ProductPrice + " and quantity of: " + item.Quantity);
            }
            sb.AppendLine("The total price is: " + TotalPrice.ToString());
            message.Content = sb.ToString();

            productInOrder.AddRange(result);

            foreach (var item in productInOrder)
            {
                this._ProductInOrderRepository.Insert(item);
            }

            loggedInUser.UserCart.ProductinShoppingCart.Clear();
            this._mailRepository.Insert(message);

            this._userRepository.Update(loggedInUser);
            return true;
        }
    }
}
