using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Orders.Models.Enums;

namespace KrMicro.Orders.CQS.Commands.Promo;

public record CreatePromoCommandRequest(string Name, string Code, decimal Value, PromoUnit PromoUnit,
    DateTimeOffset StartDate, DateTimeOffset EndDate);

public class CreatePromoCommandResult : CreateCommandResult<Models.Promo>
{
    public CreatePromoCommandResult(Models.Promo? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
    }
}