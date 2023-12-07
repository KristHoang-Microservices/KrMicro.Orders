using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Orders.Models.Enums;

namespace KrMicro.Orders.CQS.Commands.Promo;

public record UpdatePromoCommandRequest(string? Name, string? Code, decimal? Value, PromoUnit? PromoUnit,
    DateTimeOffset? StartDate, DateTimeOffset? EndDate);

public class UpdatePromoCommandResult : UpdateCommandResult<Models.Promo>
{
    public UpdatePromoCommandResult(Models.Promo? data, bool isSuccess = true, string? message = null) : base(data,
        message, isSuccess)
    {
    }
}