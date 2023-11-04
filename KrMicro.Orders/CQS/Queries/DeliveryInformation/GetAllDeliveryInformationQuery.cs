using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Orders.CQS.Queries.DeliveryInformation;

public record GetAllDeliveryInformationQueryRequest;

public class GetAllDeliveryInformationQueryResult: GetAllQueryResult<Models.DeliveryInformation>
{
    public GetAllDeliveryInformationQueryResult(List<Models.DeliveryInformation>list): base(list){}
}