﻿using KrMicro.Core.CQS.Query.Abstraction;
namespace KrMicro.Orders.CQS.Queries.Payment;

public record GetDeliveryInformationByIdQueryRequest;

public class GetDeliveryInformationByIdQueryResult : GetByIdQueryResult<Models.DeliveryInformation>
{
    public GetDeliveryInformationByIdQueryResult(Models.DeliveryInformation? data, bool isSuccess = true) : base(data, isSuccess)
    {
    }
}