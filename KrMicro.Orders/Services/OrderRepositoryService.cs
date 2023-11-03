using KrMicro.Core.Services;
using KrMicro.Orders.Infrastructure;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.Services;

public interface IOrderService : IBaseService<Order> 
{
}

public class OrderRepositoryService : BaseRepositoryService<Order, OrderDbContext>, IOrderService
{
    public OrderRepositoryService(OrderDbContext dataContext) : base(dataContext)
    {
    }
}