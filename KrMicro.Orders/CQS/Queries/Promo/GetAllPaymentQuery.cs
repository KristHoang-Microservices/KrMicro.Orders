using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Orders.CQS.Queries.Promo;

public record GetAllPromoQueryRequest;

public class GetAllPromoQueryResult : GetAllQueryResult<Models.Promo>
{
    public GetAllPromoQueryResult(List<Models.Promo> list) : base(list)
    {
    }
}