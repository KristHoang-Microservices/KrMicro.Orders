using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.CQS.Commands.OrderDetail;

public record UpdateOrderDetailStatusRequest(Status Status);

public class UpdateOrderDetailStatusCommandResult : UpdateStatusCommandResult
{
    public UpdateOrderDetailStatusCommandResult(string message, bool isSuccess = true) : base(message, isSuccess)
    {
    }
}