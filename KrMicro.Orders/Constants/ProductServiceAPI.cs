namespace KrMicro.Orders.Constants;

public static class ProductServiceAPI
{
    // private static readonly string ApiBase = "https://localhost:7127/api";
    private static readonly string ApiBase = "https://krmicro-masterdata.azurewebsites.net/api";

    public static readonly string GetProduct = $"{ApiBase}/Product";
    public static readonly string UpdateStock = "/UpdateStock";

    public static string GetProductByIdAndSize(short id, string sizeCode)
    {
        return $"{GetProduct}/{id}/{sizeCode}";
    }

    public static string UpdateStockById(short id)
    {
        return $"{ApiBase}/{id}/{UpdateStock}";
    }
}