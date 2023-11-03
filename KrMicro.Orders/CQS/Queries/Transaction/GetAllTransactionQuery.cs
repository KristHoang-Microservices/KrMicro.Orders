using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Orders.CQS.Queries.Transaction;

public record GetAllTransactionQueryRequest;

public class GetAllTransactionQueryResult: GetAllQueryResult<Models.Transaction>
{
    public GetAllTransactionQueryResult(List<Models.Transaction>list): base(list){}
}