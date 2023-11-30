using KrMicro.Core.CQS.Query.Abstraction;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Models.Enums;

namespace KrMicro.Orders.CQS.Queries.Order;

public record GetAllOrderQueryRequest(OrderStatus? OrderStatus, Status? Status, DateTimeOffset? FromDate,
    DateTimeOffset? ToDate);

public class GetAllOrderQueryResult : GetAllQueryResult<Models.Order>
{
    public GetAllOrderQueryResult(List<Models.Order> list) : base(list)
    {
    }
}