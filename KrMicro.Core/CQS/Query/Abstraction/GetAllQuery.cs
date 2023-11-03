namespace KrMicro.Core.CQS.Query.Abstraction;

public class GetAllQueryResult<TEntity>
{
    protected GetAllQueryResult(List<TEntity> list)
    {
        ListData = list;
    }

    public List<TEntity> ListData { get; set; }
}