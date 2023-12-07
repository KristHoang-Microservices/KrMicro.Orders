namespace KrMicro.Orders.CQS.Commands.Transaction;

public record CreateVnPayTransactionCommandRequest(string PhoneNumber, short OrderId, short PaymentMethodId);

public class CreateVnPayTransactionCommandResult
{
    public CreateVnPayTransactionCommandResult(string paymentUrl, short transactionId)
    {
        PaymentUrl = paymentUrl;
        TransactionId = transactionId;
    }

    public string PaymentUrl { get; set; }
    public short TransactionId { get; set; }
}