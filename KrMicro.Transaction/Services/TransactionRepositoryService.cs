using KrMicro.Core.Services;
using KrMicro.Transaction.Infrastructure;
using KrMicro.Transaction.Models;

namespace KrMicro.Transaction.Services;

public interface ITransactionService : IBaseService<Models.Transaction>
{
}

public class TransactionRepositoryService : BaseRepositoryService<Models.Transaction, TransactionDbContext>, ITransactionService
{
    public TransactionRepositoryService(TransactionDbContext dataContext) : base(dataContext)
    {
    }
}