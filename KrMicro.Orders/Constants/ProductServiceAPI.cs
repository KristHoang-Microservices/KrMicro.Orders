namespace KrMicro.Orders.Constants;

public static class ProductServiceAPI
{
    public static string ApiBase = "https://krmicro-masterdata.azurewebsites.net/api";

    public static string GetProduct = $"{ApiBase}/ProductPrice";
    public static string GetProductByIdAndSize(short id, string sizeCode)
    {
        return $"{ApiBase}/{id}?sizeCode={sizeCode}";
    }
}