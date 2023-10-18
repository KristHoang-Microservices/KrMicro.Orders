﻿using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Transaction.CQS.Queries.Payment;

public record GetAllPaymentQueryRequest;

public class GetAllPaymentQueryResult: GetAllQueryResult<Models.Payment>
{
    public GetAllPaymentQueryResult(List<Models.Payment>list): base(list){}
}