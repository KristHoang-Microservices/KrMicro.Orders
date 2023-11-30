using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Order;

public record CheckOrderCommandRequest(List<OrderDetailRequest> OrderDetails);

public record FaultProductQuantity(string SizeCode, short ProductId);

public class CheckOrderCommandResult : UpdateStatusCommandResult
{
    public CheckOrderCommandResult(string message, List<FaultProductQuantity> data, bool isSuccess = true) : base(
        message,
        isSuccess)
    {
        Data = data;
    }

    public List<FaultProductQuantity> Data { get; set; }
}