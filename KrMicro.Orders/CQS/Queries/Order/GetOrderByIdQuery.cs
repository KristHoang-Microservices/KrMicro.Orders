using KrMicro.Core.CQS.Query.Abstraction;
namespace KrMicro.Orders.CQS.Queries.Payment;

public record GetOrderByIdQueryRequest;

public class GetOrderByIdQueryResult : GetByIdQueryResult<Models.Order>
{
    public GetOrderByIdQueryResult(Models.Order? data, bool isSuccess = true) : base(data, isSuccess)
    {
    }
}