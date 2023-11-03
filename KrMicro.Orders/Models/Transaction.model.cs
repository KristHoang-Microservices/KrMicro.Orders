using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.Models;

[Table("Transactions")]
public class Transaction : BaseModelWithAuditAndTracking
{
    [Column("CustomerId")] public short CustomerId { get; set; }
    [Required] [Column("PhoneNumber")] public string PhoneNumber { get; set; }

    [Required] [Column("OrderId")] public short OrderId { get; set; }

    [ForeignKey("PaymentId")] public short PaymentId { get; set; }

    public Payment Payment { get; set; }

    public Order Order { get; set; }
}