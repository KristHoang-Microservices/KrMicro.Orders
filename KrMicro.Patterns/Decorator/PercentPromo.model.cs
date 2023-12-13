namespace KrMicro.Patterns.Decorator;

public class PercentPromo : PromoDecorator, IPromo
{
    public PercentPromo(IPromo promo) : base(promo)
    {
    }

    public override decimal CalculateDueToPrice(decimal orderTotal)
    {
        return orderTotal * (1 - Value);
    }
}