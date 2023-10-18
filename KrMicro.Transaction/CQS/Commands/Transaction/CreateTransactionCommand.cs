using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Transaction.CQS.Commands.Transaction;

public record CreateTransactionCommandRequest(string? CustomerId, string PhoneNumber, string OrderId, string PaymentId);

public class CreateTransactionCommandResult : CreateCommandResult<Models.Transaction>
{
    public CreateTransactionCommandResult(Models.Transaction? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}