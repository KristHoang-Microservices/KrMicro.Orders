using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record UpdatePaymentCommandRequest(string Name);

public class UpdatePaymentCommandResult : UpdateCommandResult<PaymentMethod>
{
    public UpdatePaymentCommandResult(PaymentMethod? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
    }
}