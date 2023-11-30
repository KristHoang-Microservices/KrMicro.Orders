using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Models.Enums;

namespace KrMicro.Orders.Models;

[Table("Transactions")]
public class Transaction : BaseModelWithAuditAndTracking
{
    [Column("CustomerId")] public short? CustomerId { get; set; }
    [Column("CustomerName")] public string? CustomerName { get; set; }
    [Required] [Column("PhoneNumber")] public string PhoneNumber { get; set; } = string.Empty;
    [Required] [Column("Total")] public decimal Total { get; set; }

    [Required] [Column("OrderId")] public short OrderId { get; set; }

    [ForeignKey("PaymentMethodId")] public short PaymentMethodId { get; set; }

    [Column("TransactionStatus")] public TransactionStatus TransactionStatus { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public Order Order { get; set; }
}