namespace KrMicro.Orders.Constants;

public static class ClientSideHost
{
    private static readonly string BaseUrl = "http://localhost:3000";

    public static string GetReturnUrl(int orderId)
    {
        return BaseUrl + $"/orders/pay/success/{orderId}";
    }
}