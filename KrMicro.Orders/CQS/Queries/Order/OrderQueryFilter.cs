using System.Linq.Expressions;
using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Orders.CQS.Queries.Order;

public class OrderQueryFilter : AbstractFilter<Models.Order>
{
    private readonly GetAllOrderQueryRequest _request;

    public OrderQueryFilter(GetAllOrderQueryRequest request)
    {
        _request = request;
    }

    protected override List<Expression<Func<Models.Order, bool>>> AddRequestToExpression(Models.Order data)
    {
        var exp = new List<Expression<Func<Models.Order, bool>>>
        {
            x => !_request.OrderStatus.HasValue || data.OrderStatus == _request.OrderStatus,
            x => !_request.Status.HasValue || data.Status == _request.Status,
            x => !_request.FromDate.HasValue || data.CreatedAt!.Value.CompareTo(_request.FromDate.Value) >= 0,
            x => !_request.ToDate.HasValue || data.CreatedAt!.Value.CompareTo(_request.ToDate.Value) <= 0
        };
        return exp;
    }
}