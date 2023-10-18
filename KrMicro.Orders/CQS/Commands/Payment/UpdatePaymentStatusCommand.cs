using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record UpdatePaymentStatusRequest(Status Status);

public class UpdatePaymentStatusCommandResult : UpdateStatusCommandResult
{
    public UpdatePaymentStatusCommandResult(string message, bool isSuccess = true) : base(message, isSuccess)
    {
    }
}