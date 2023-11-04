using Newtonsoft.Json;

namespace KrMicro.Orders.Models.Api;

public class LoginRequest
{
    [JsonConstructor]
    public LoginRequest(string username, string password)
    {
        this.username = username;
        this.password = password;
    }

    public string username { get; set; }
    public string password { get; set; }
}