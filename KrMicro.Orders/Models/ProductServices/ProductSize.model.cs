using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using KrMicro.Core.Models.Abstraction;
using Newtonsoft.Json;

namespace KrMicro.Orders.Models.ProductServices;

public class ProductSize : BaseModelWithAuditAndTracking
{
[JsonConstructor]
    public ProductSize(short sizeId, short productId, short stock, decimal price)
    {
        this.sizeId = sizeId;
        this.productId = productId;
        this.stock = stock;
        this.price = price;
    }

    public short sizeId {get; set;}
    public short productId {get; set;}
    public short stock {get; set;}
    public decimal price {get; set;}
}