using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Models.Enums;

namespace KrMicro.Orders.Models;

[Table("Orders")]
public class Order : BaseModelWithAuditAndTracking
{
    [Column("OrderDate")] public DateTimeOffset? OrderDate { get; set; }

    [Column("Total")] public decimal Total { get; set; }

    [Column("OrderStatus")] public OrderStatus OrderStatus { get; set; }

    [ForeignKey("DeliveryInformationId")] public short DeliveryInformationId { get; set; }

    [ForeignKey("PromoId")] public short? PromoId { get; set; }

    [Column("Note")] public string? Note { get; set; } = string.Empty;

    public List<OrderDetail> OrderDetails { get; set; }

    public DeliveryInformation DeliveryInformation { get; set; }

    public List<Transaction> Transactions { get; set; }

    public Promo? Promo { get; set; }
}