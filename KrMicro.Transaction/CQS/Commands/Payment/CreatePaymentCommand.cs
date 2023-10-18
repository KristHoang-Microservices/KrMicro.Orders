using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Transaction.CQS.Commands.Payment;

public record CreatePaymentCommandRequest(string Name);

public class CreatePaymentCommandResult : CreateCommandResult<Models.Payment>
{
    public CreatePaymentCommandResult(Models.Payment? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
        
    }
}