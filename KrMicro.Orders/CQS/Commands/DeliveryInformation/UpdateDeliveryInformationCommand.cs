using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record UpdateDeliveryInformationCommandRequest(string Name, string FullAddress, string Phone);

public class UpdateDeliveryInformationCommandResult : UpdateCommandResult<Models.DeliveryInformation>
{
    public UpdateDeliveryInformationCommandResult(Models.DeliveryInformation? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}