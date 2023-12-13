using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Models;
using KrMicro.Orders.Models.Enums;

namespace KrMicro.Patterns.Factory;

[Table("Transactions")]
public abstract class AbstractTransaction : BaseModelWithAuditAndTracking
{
    [Column("CustomerId")] public short? CustomerId { get; set; }
    [Column("CustomerName")] public string? CustomerName { get; set; }
    [Required] [Column("PhoneNumber")] public string PhoneNumber { get; set; } = string.Empty;
    [Required] [Column("Total")] public decimal Total { get; set; }

    [Required] [Column("OrderId")] public short OrderId { get; set; }

    [Column("TransactionStatus")] public TransactionStatus TransactionStatus { get; set; }

    public Order Order { get; set; }
}