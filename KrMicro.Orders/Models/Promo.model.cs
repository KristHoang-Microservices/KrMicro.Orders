using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Models.Enums;

namespace KrMicro.Orders.Models;

[Table("Promo")]
public class Promo : BaseModelWithAuditAndTracking
{
    [Required] [Column("Name")] public string Name { get; set; } = string.Empty;
    [Required] [Column("Code")] public string Code { get; set; } = string.Empty;
    [Required] [Column("Value")] public decimal Value { get; set; }
    [Required] [Column("PromoUnit")] public PromoUnit PromoUnit { get; set; } = PromoUnit.Raw;
    [Required] [Column("StartDate")] public DateTimeOffset StartDate { get; set; }
    [Required] [Column("EndDate")] public DateTimeOffset EndDate { get; set; }
}