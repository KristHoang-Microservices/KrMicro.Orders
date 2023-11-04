using Newtonsoft.Json;

namespace KrMicro.Orders.CQS.Queries.Api.Customer;

public class GetCustomerDetailQueryResult
{
    [JsonConstructor]
    public GetCustomerDetailQueryResult(short? id, string userId, int point, DateTime dob, string name, string phone)
    {
        this.id = id;
        this.userId = userId;
        this.point = point;
        this.dob = dob;
        this.name = name;
        this.phone = phone;
    }

    public short? id { get; set; }

    public string userId { get; set; }

    public int point { get; set; }

    public string fullAddress { get; set; } = string.Empty;

    public DateTime dob { get; set; }

    public string name { get; set; }

    public string phone { get; set; }
}