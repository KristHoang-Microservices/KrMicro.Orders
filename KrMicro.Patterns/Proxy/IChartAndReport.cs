namespace KrMicro.Patterns.Proxy;

public interface IChartAndReport<T>
{
    public Task<T> GetResult();
}