using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShop.Domain.DomainModels;
using EShop.Repository.Interface;
using EShop.Service.Interface;

namespace EShop.Service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public List<Order> GetAllOrders()
        {
            return orderRepository.getAllOrders();
        }

        public Order GetOrderDetails(BaseEntity model)
        {
            return orderRepository.GetOrderDetails(model);
        }
    }
}
