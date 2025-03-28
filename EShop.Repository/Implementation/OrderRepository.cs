using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eshop.Web.Data;
using EShop.Domain.DomainModels;
using EShop.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace EShop.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {

        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }

        public List<Order> getAllOrders()
        {
            return entities
                .Include(z=>z.Products)
                .Include("Products.SelectedProduct")
                .Include(z=>z.User)
                .ToList();
        }

        public Order GetOrderDetails(BaseEntity model)
        {
            return entities
              .Include(z => z.Products)
              .Include(z => z.User)
              .Include("Products.SelectedProduct")
              .SingleOrDefaultAsync(s => s.Id == model.Id).Result;
        }
    }
}
