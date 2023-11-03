using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Order;

public record UpdateOrderCommandRequest( short? DeliveryInformationId, List<OrderDetailRequest> OrderDetails, string? Note);

public class UpdateOrderCommandResult : UpdateCommandResult<Models.Order>
{
    public UpdateOrderCommandResult(Models.Order? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}