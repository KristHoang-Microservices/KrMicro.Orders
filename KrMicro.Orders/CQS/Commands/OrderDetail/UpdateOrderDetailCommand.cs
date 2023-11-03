using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record UpdateOrderDetailCommandRequest( short? ProductId);

public class UpdateOrderDetailCommandResult : UpdateCommandResult<Models.OrderDetail>
{
    public UpdateOrderDetailCommandResult(Models.OrderDetail? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}