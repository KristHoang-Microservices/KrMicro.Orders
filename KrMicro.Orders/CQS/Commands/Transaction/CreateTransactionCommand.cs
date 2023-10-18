using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Transaction;

public record CreateTransactionCommandRequest(short CustomerId, string PhoneNumber, short OrderId, short PaymentId);

public class CreateTransactionCommandResult : CreateCommandResult<Models.Transaction>
{
    public CreateTransactionCommandResult(Models.Transaction? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}