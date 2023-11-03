using System.Text.Json.Serialization;

namespace KrMicro.Core.CQS.Query.Abstraction;

public class GetByIdQueryResult<TEntity>
{
    [JsonConstructor]
    public GetByIdQueryResult(TEntity? data, bool isSuccess)
    {
        this.data = data;
        this.isSuccess = isSuccess;
    }

    public TEntity? data { get; set; }
    public bool isSuccess { get; set; }
}