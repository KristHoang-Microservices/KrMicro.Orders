namespace KrMicro.Orders.Constants;

public class NetworkFailedResponse
{
    public static readonly string NotFound = "Giving request not found";
    public static readonly string BadRequest = "Failed to process due to lack information of giving request";

    public static string FailedToProcess(string entityName)
    {
        return entityName + " was failed to process";
    }
}