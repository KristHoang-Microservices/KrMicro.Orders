using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record CreateDeliveryInformationCommandRequest(string Name, string FullAddress, string Phone);

public class CreateDeliveryInformationCommandResult : CreateCommandResult<Models.DeliveryInformation>
{
    public CreateDeliveryInformationCommandResult(Models.DeliveryInformation? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}