using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Orders.CQS.Queries.Payment;

public record GetAllDeliveryInformationQueryRequest;

public class GetAllDeliveryInformationQueryResult: GetAllQueryResult<Models.DeliveryInformation>
{
    public GetAllDeliveryInformationQueryResult(List<Models.DeliveryInformation>list): base(list){}
}