using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Transaction;

public record UpdateTransactionCommandRequest(short? CustomerId, string PhoneNumber, short? OrderId, short? PaymentId);

public class UpdateTransactionCommandResult : UpdateCommandResult<Models.Transaction>
{
    public UpdateTransactionCommandResult(Models.Transaction? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}