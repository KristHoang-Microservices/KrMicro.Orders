using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Transaction.CQS.Commands.Transaction;

public record UpdateTransactionCommandRequest(string? CustomerId, string PhoneNumber, string OrderId, string PaymentId);

public class UpdateTransactionCommandResult : UpdateCommandResult<Models.Transaction>
{
    public UpdateTransactionCommandResult(Models.Transaction? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}