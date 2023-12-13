namespace KrMicro.Patterns.Iterator;

public class OrderDetailIterator : IAbstractIterator<OrderDetail>
{
    private readonly OrderDetailCollection collection;
    private int current;

    public OrderDetailIterator(OrderDetailCollection collection)
    {
        this.collection = collection;
    }

    public OrderDetail First()
    {
        current = 0;
        return collection[current];
    }

    public OrderDetail Next()
    {
        current += Step;
        if (!IsDone) return collection[current];
        return null;
    }

    public int Step { get; set; } = 1;

    public OrderDetail CurrentItem => collection[current];

    public bool IsDone => current >= collection.Count;
}