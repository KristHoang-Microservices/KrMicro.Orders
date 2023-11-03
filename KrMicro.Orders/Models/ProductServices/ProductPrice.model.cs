using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.Models.ProductServices;

public class ProductPrice : BaseModelWithAuditAndTracking
{
    public short Id;
    public decimal price;
}