namespace KrMicro.Patterns.Iterator;

public class OrderDetailCollection : IAbstractCollection<OrderDetail>
{
    private readonly List<OrderDetail> _details = new();

    public int Count => _details.Count;

    public OrderDetail this[int index]
    {
        get => _details[index];
        set => _details.Add(value);
    }

    public IAbstractIterator<OrderDetail> CreateIterator()
    {
        return new OrderDetailIterator(this);
    }
}