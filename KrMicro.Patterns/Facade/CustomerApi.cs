using KrMicro.Orders.CQS.Queries.Api.Customer;

namespace KrMicro.Patterns.Facade;

public class CustomerApi
{
    private static readonly string ApiBase = "https://krmicro-identity.azurewebsites.net/api";

    private static readonly string GetAllCustomer = $"{ApiBase}/Customers";
    private readonly ApiFacade _apiFacade = ApiFacade.GetInstance();

    private static string GetCustomerById(short id)
    {
        return $"{GetAllCustomer}/{id}";
    }

    public async Task<GetCustomerDetailQueryResult?> GetCustomerDetail(short customerId)
    {
        return await _apiFacade.Get<GetCustomerDetailQueryResult>(GetCustomerById(customerId));
    }
}