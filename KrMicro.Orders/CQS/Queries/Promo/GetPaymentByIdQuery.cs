using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Orders.CQS.Queries.Promo;

public record GetPromoByIdQueryRequest;

public class GetPromoByIdQueryResult : GetByIdQueryResult<Models.Promo>
{
    public GetPromoByIdQueryResult(Models.Promo? data, bool isSuccess = true) : base(data, isSuccess)
    {
    }
}