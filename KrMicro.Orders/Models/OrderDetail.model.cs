using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.Models;

[Table("OrderDetail")]
public class OrderDetail : NoIdWithAuditAndTracking
{
    [Required] [Column("ProductId")] public short ProductId { get; set; }
    
    [Required] [Column("OrderId")] public short OrderId { get; set; }

    [Column("Quantity")] public int Quantity { get; set; }
    
    [Column("SizeCode")] public string SizeCode { get; set; }

    [Column("Price")] public decimal Price { get; set; }

    public Order Order { get; set; }
}