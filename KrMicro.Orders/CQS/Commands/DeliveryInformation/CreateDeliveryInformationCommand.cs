using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Orders.CQS.Commands.DeliveryInformation;

public record CreateDeliveryInformationCommandRequest(string Name, string CustomerName, string FullAddress,
    string Phone, short? CustomerId);

public class CreateDeliveryInformationCommandResult : CreateCommandResult<Models.DeliveryInformation>
{
    public CreateDeliveryInformationCommandResult(Models.DeliveryInformation? data, bool isSuccess = true,
        string? message = null) : base(data,
        message, isSuccess)
    {
    }
}