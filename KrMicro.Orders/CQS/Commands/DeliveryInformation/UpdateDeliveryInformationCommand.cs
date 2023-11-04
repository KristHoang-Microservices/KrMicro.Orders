using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.DeliveryInformation;

public record UpdateDeliveryInformationCommandRequest(string? Name, string? CustomerName, string? FullAddress,
    string? Phone);

public class UpdateDeliveryInformationCommandResult : UpdateCommandResult<Models.DeliveryInformation>
{
    public UpdateDeliveryInformationCommandResult(Models.DeliveryInformation? data, bool isSuccess = true,
        string? message = null) : base(data,
        message, isSuccess)
    {
    }
}