using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record UpdateOrderStatusRequest(Status Status);

public class UpdateOrderStatusCommandResult : UpdateStatusCommandResult
{
    public UpdateOrderStatusCommandResult(string message, bool isSuccess = true) : base(message, isSuccess)
    {
    }
}