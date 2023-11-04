using System.Net.Http.Headers;
using System.Text.Json;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.Api;
using KrMicro.Orders.Models.Api;

namespace KrMicro.Orders.Services;

public static class ApiHttpClient
{
    private const string Password = "Orders@123";
    private const string Username = "orderservice";
    private static string _accessToken = string.Empty;

    public static async Task<HttpClient> GetHttpClient()
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
}