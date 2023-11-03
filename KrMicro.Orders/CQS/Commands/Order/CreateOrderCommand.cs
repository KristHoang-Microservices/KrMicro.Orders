using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record OrderDetailRequest(short ProductId, short Quantity, string SizeCode);

public record CreateOrderCommandRequest( short DeliveryInformationId, List<OrderDetailRequest> OrderDetails);

public class CreateOrderCommandResult : CreateCommandResult<Models.Order>
{
    public CreateOrderCommandResult(Models.Order? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}