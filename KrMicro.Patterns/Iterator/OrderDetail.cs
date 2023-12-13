namespace KrMicro.Patterns.Iterator;

public class OrderDetail
{
    private string _sizeCode;

    public OrderDetail(short productId, short amount, string sizeCode)
    {
        ProductId = productId;
        Amount = amount;
        _sizeCode = sizeCode;
    }

    public short Amount { get; set; }

    public short ProductId { get; set; }

    public string SizeCode
    {
        get => _sizeCode;
        set => _sizeCode = value ?? throw new ArgumentNullException(nameof(value));
    }
}