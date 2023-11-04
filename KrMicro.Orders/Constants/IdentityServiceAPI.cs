namespace KrMicro.Orders.Constants;

public static class IdentityServiceAPI
{
    // private static readonly string ApiBase = "https://localhost:7127/api";
    public static readonly string ApiBase = "https://krmicro-identity.azurewebsites.net/api";

    public static readonly string Login = $"{ApiBase}/Identity/Login";
}