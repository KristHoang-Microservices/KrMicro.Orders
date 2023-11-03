using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Orders.CQS.Queries.Payment;

public record GetAllOrderQueryRequest;

public class GetAllOrderQueryResult: GetAllQueryResult<Models.Order>
{
    public GetAllOrderQueryResult(List<Models.Order>list): base(list){}
}