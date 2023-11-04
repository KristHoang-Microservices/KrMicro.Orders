using KrMicro.Core.Services;
using KrMicro.Orders.Infrastructure;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.Services;

public interface IPaymentService : IBaseService<PaymentMethod>
{
}

public class PaymentRepositoryService : BaseRepositoryService<PaymentMethod, OrderDbContext>, IPaymentService
{
    public PaymentRepositoryService(OrderDbContext dataContext) : base(dataContext)
    {
    }
}