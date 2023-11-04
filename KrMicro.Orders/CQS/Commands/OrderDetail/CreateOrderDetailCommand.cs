using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.OrderDetail;

public record CreateOrderDetailCommandRequest( short? ProductId);

public class CreateOrderDetailCommandResult : CreateCommandResult<Models.OrderDetail>
{
    public CreateOrderDetailCommandResult(Models.OrderDetail? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}