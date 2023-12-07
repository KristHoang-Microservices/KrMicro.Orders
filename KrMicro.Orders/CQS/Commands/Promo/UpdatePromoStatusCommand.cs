using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.CQS.Commands.Promo;

public record UpdatePromoStatusRequest(Status Status);

public class UpdatePromoStatusCommandResult : UpdateStatusCommandResult
{
    public UpdatePromoStatusCommandResult(string message, bool isSuccess = true) : base(message, isSuccess)
    {
    }
}