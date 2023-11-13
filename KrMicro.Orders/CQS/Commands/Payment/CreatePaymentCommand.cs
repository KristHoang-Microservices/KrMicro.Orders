using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.CQS.Commands.Payment;

public record CreatePaymentCommandRequest(string Name, string? Description);

public class CreatePaymentCommandResult : CreateCommandResult<PaymentMethod>
{
    public CreatePaymentCommandResult(PaymentMethod? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
    }
}