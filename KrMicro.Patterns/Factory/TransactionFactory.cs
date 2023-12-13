namespace KrMicro.Patterns.Factory;

public class TransactionFactory
{
    public enum TransactionType
    {
        Vnpay,
        Cash
    }

    public AbstractTransaction CreateTransaction(TransactionType type)
    {
        switch (type)
        {
            case TransactionType.Vnpay:
                return new VnPayTransaction();
            case TransactionType.Cash:
            default:
                return new CashTransaction();
        }
    }
}