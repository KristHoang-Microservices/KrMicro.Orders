using KrMicro.Core.CQS.Query.Abstraction;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.CQS.Queries.Payment;

public record GetPaymentByIdQueryRequest;

public class GetPaymentByIdQueryResult : GetByIdQueryResult<PaymentMethod>
{
    public GetPaymentByIdQueryResult(PaymentMethod? data, bool isSuccess = true) : base(data, isSuccess)
    {
    }
}