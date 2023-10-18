using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Transaction.Models;

[Table("Transactions")]

public class Transaction : BaseModelWithAuditAndTracking
{

     [Column("CustomerId")] public string? CustomerId { get; set; }
     [Required] [Column("PhoneNumber")] public string PhoneNumber { get; set; }

     [Column("OrderId")] public string OrderId { get; set; }

     [ForeignKey("PaymentId")] public string PaymentId { get; set; }
}