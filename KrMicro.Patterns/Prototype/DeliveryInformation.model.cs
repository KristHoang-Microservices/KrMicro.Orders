using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Models;

namespace KrMicro.Patterns.Prototype;

[Table("DeliveryInformation")]
public class DeliveryInformation : BaseModelWithAuditAndTracking, IPrototype<DeliveryInformation>
{
    [Column("Name")] public string Name { get; set; }

    [Column("FullAddress")] public string FullAddress { get; set; }

    [Column("Phone")] public string Phone { get; set; }

    [Column("CustomerName")] public string CustomerName { get; set; }
    [Column("CityId")] public short CityId { get; set; }
    [Column("DistrictId")] public short DistrictId { get; set; }
    [Column("WardId")] public short WardId { get; set; }

    [ForeignKey("CustomerId")] public short? CustomerId { get; set; }

    public List<Order> Orders { get; set; }

    public DeliveryInformation ShallowCopy()
    {
        return (DeliveryInformation)MemberwiseClone();
    }

    public DeliveryInformation DeepCopy()
    {
        var clone = ShallowCopy();
        clone.Orders = new List<Order>(Orders);
        return clone;
    }
}