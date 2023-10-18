using KrMicro.Core.Services;
using KrMicro.Transaction.Infrastructure;
using KrMicro.Transaction.Models;

namespace KrMicro.Transaction.Services;

public interface IPaymentService : IBaseService<Payment>
{
}

public class PaymentRepositoryService : BaseRepositoryService<Payment, TransactionDbContext>, IPaymentService
{
    public PaymentRepositoryService(TransactionDbContext dataContext) : base(dataContext)
    {
    }
}