using KrMicro.Core.CQS.Query.Abstraction;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Models.Enums;

namespace KrMicro.Orders.CQS.Queries.Transaction;

public record GetAllTransactionQueryRequest(TransactionStatus? TransactionStatus, Status? Status,
    DateTimeOffset? FromDate,
    DateTimeOffset? ToDate);

public class GetAllTransactionQueryResult : GetAllQueryResult<Models.Transaction>
{
    public GetAllTransactionQueryResult(List<Models.Transaction> list) : base(list)
    {
    }
}