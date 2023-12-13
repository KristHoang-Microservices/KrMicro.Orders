namespace KrMicro.Patterns.Decorator;

public abstract class PromoDecorator : IPromo
{
    protected IPromo Promo;

    protected PromoDecorator(IPromo promo)
    {
        Promo = promo;
    }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public abstract decimal CalculateDueToPrice(decimal orderTotal);
}