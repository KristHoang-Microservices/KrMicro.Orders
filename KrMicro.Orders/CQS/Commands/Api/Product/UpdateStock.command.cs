using KrMicro.Orders.Models.Api.Products;

namespace KrMicro.Orders.CQS.Commands.Api.Product;

public class UpdateStockCommand
{
    public List<ProductSizes> ProductSizes = new();
}