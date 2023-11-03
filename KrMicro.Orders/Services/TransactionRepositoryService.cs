using KrMicro.Core.Services;
using KrMicro.Orders.Infrastructure;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.Services;

public interface ITransactionService : IBaseService<Transaction>
{
}

public class TransactionRepositoryService : BaseRepositoryService<Transaction, OrderDbContext>, ITransactionService
{
    public TransactionRepositoryService(OrderDbContext dataContext) : base(dataContext)
    {
    }
}