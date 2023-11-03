using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record UpdateOrderCommandRequest( short? DeliveryInformationId);

public class UpdateOrderCommandResult : UpdateCommandResult<Models.Order>
{
    public UpdateOrderCommandResult(Models.Order? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}