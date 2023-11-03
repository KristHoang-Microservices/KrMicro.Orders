using KrMicro.Core.Services;
using KrMicro.Orders.Infrastructure;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.Services;

public interface IPaymentService : IBaseService<Payment>
{
}

public class PaymentRepositoryService : BaseRepositoryService<Payment, OrderDbContext>, IPaymentService
{
    public PaymentRepositoryService(OrderDbContext dataContext) : base(dataContext)
    {
    }
}