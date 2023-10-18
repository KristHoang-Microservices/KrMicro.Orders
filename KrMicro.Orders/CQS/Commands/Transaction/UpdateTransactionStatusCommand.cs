using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Transaction;

public record UpdateTransactionStatusRequest(Status Status);

public class UpdateTransactionStatusCommandResult : UpdateStatusCommandResult
{
    public UpdateTransactionStatusCommandResult(string message, bool isSuccess = true) : base(message, isSuccess)
    {
    }
}