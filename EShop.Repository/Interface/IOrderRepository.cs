using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShop.Domain.DomainModels;

namespace EShop.Repository.Interface
{
    public interface IOrderRepository
    {
        List<Order> getAllOrders();
        Order GetOrderDetails(BaseEntity model);

    }
}
