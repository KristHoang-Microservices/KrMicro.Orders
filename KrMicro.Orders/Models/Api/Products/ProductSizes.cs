using System.Text.Json.Serialization;

namespace KrMicro.Orders.Models.Api.Products;

public class ProductSizes
{
    [JsonConstructor]
    public ProductSizes(int stock)
    {
        this.stock = stock;
    }

    public int stock { get; set; }
}