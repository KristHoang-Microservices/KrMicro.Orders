using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.Models;

[Table("DeliveryInformation")]
public class DeliveryInformation : BaseModelWithAuditAndTracking
{
    [Column("Name")] public string Name { get; set; }

    [Column("FullAddress")] public string FullAddress { get; set; }

    [Column("Phone")] public string Phone { get; set; }

    [ForeignKey("CustomerId")] public short? CustomerId { get; set; }

    public List<Order> Orders { get; set; }
}