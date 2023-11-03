using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Order;

public record OrderDetailRequest(short ProductId, short Amount, string SizeCode);

public record CreateOrderCommandRequest( short DeliveryInformationId, List<OrderDetailRequest> OrderDetails, string? Note);

public class CreateOrderCommandResult : CreateCommandResult<Models.Order>
{
    public CreateOrderCommandResult(Models.Order? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}