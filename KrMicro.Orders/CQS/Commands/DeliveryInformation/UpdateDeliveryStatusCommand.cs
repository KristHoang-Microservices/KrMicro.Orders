using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record UpdateDeliveryInformationStatusRequest(Status Status);

public class UpdateDeliveryInformationStatusCommandResult : UpdateStatusCommandResult
{
    public UpdateDeliveryInformationStatusCommandResult(string message, bool isSuccess = true) : base(message, isSuccess)
    {
    }
}