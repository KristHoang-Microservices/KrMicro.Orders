using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.Api;
using KrMicro.Orders.Models.Api;

namespace KrMicro.Patterns.Facade;

public class ApiFacade
{
    private const string Password = "Orders@123";
    private const string Username = "orderservice";
    private static string _accessToken = string.Empty;
    private static ApiFacade? _instance;

    private ApiFacade()
    {
    }

    public static ApiFacade GetInstance()
    {
        return _instance ??= new ApiFacade();
    }

    private static async Task<HttpClient> GetHttpClient()
    {
        var client = new HttpClient();

        if (_accessToken == string.Empty)
        {
            var res = await client.PostAsync(IdentityServiceAPI.Login,
                JsonContent.Create(new LoginRequest(Username, Password)));

            if (res.IsSuccessStatusCode)
            {
                var value = JsonSerializer.Deserialize<LoginCommandResult>(await res.Content.ReadAsStringAsync());
                _accessToken = value?.accessToken ?? string.Empty;
            }
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        return client;
    }

    public async Task<TResponse?> Get<TResponse>(string uri)
    {
        var client = await GetHttpClient();
        var res = await client.GetAsync(uri);

        if (res.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<TResponse>(await res.Content.ReadAsStringAsync());

        return default;
    }

    public async Task<List<TResponse>?> GetAll<TResponse>(string uri)
    {
        var client = await GetHttpClient();
        var res = await client.GetAsync(uri);

        if (res.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<List<TResponse>>(await res.Content.ReadAsStringAsync());

        return new List<TResponse>();
    }

    public async Task<TResponse?> Post<TResponse, TRequest>(string uri, TRequest request)
    {
        var client = await GetHttpClient();
        var res = await client.PostAsync(uri, JsonContent.Create(request));

        if (res.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<TResponse>(await res.Content.ReadAsStringAsync());

        return default;
    }

    public async Task<List<TResponse>?> PostAll<TResponse, TRequest>(string uri, List<TRequest> requests)
    {
        var client = await GetHttpClient();
        var data = new List<object>();

        foreach (var request in requests) data.Add(JsonContent.Create(request));

        var res = await client.PostAsync(uri, JsonContent.Create(data));

        if (res.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<List<TResponse>>(await res.Content.ReadAsStringAsync());

        return new List<TResponse>();
    }
}