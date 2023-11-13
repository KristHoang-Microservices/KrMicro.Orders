using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.Models;

[Table("PaymentMethods")]
public class PaymentMethod : BaseModelWithAuditAndTracking
{
    public List<Transaction> Transactions;
    [Required] [Column("Name")] public string Name { get; set; } = string.Empty;
    [Column("Description")] public string? Description { get; set; } = string.Empty;
}