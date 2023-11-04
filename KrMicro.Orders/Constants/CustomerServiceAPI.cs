namespace KrMicro.Orders.Constants;

public static class CustomerServiceAPI
{
    // private static readonly string ApiBase = "https://localhost:7127/api";
    public static readonly string ApiBase = "https://krmicro-identity.azurewebsites.net/api";

    public static readonly string GetAllCustomer = $"{ApiBase}/Customers";

    public static string GetCustomerById(short id)
    {
        return $"{GetAllCustomer}/{id}";
    }
}