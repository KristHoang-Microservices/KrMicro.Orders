using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Orders.CQS.Queries.Order;

public record GetAllOrderQueryRequest;

public class GetAllOrderQueryResult: GetAllQueryResult<Models.Order>
{
    public GetAllOrderQueryResult(List<Models.Order>list): base(list){}
}