using KrMicro.Core.CQS.Query.Abstraction;
namespace KrMicro.Orders.CQS.Queries.Payment;

public record GetPaymentByIdQueryRequest;

public class GetPaymentByIdQueryResult : GetByIdQueryResult<Models.Payment>
{
    public GetPaymentByIdQueryResult(Models.Payment? data, bool isSuccess = true) : base(data, isSuccess)
    {
    }
}