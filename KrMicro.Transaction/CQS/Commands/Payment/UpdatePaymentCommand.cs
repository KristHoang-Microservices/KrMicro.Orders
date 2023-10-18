using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Transaction.CQS.Commands.Payment;

public record UpdatePaymentCommandRequest(string Name);

public class UpdatePaymentCommandResult : UpdateCommandResult<Models.Payment>
{
    public UpdatePaymentCommandResult(Models.Payment? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}