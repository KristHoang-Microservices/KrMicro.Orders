using KrMicro.Core.CQS.Query.Abstraction;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.CQS.Queries.Payment;

public record GetAllPaymentQueryRequest;

public class GetAllPaymentQueryResult : GetAllQueryResult<PaymentMethod>
{
    public GetAllPaymentQueryResult(List<PaymentMethod> list) : base(list)
    {
    }
}