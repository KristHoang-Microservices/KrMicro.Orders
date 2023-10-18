using KrMicro.Core.Services;
using KrMicro.Orders.Infrastructure;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.Services;

public interface ITransactionService : IBaseService<Models.Transaction>
{
}

public class TransactionRepositoryService : BaseRepositoryService<Models.Transaction, TransactionDbContext>, ITransactionService
{
    public TransactionRepositoryService(TransactionDbContext dataContext) : base(dataContext)
    {
    }
}