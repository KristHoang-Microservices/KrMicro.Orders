using KrMicro.Core.Services;
using KrMicro.Orders.Infrastructure;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.Services;

public interface IOrderDetailService : IBaseService<OrderDetail>
{
}

public class OrderDetailRepositoryService : BaseRepositoryService<OrderDetail, OrderDbContext>, IOrderDetailService
{
    public OrderDetailRepositoryService(OrderDbContext dataContext) : base(dataContext)
    {
    }
}