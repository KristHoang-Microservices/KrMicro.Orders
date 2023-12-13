using KrMicro.Orders.Infrastructure;
using KrMicro.Orders.Models.Enums;
using KrMicro.Orders.Services;

namespace KrMicro.Patterns.Proxy;

public class IncomeChartService
{
    private static readonly IOrderService OrderService = new OrderRepositoryService(new OrderDbContext());
    private static readonly List<IncomeChartProxy> Charts = new();

    public static async Task<IncomeChart?> GetByTimeRange(TimeRange timeRange)
    {
        var chartProxy = Charts.Find(x => x.TimeRange == timeRange);
        if (chartProxy == null)
        {
            chartProxy = new IncomeChartProxy(timeRange, OrderService);
            Charts.Add(chartProxy);
        }

        return await chartProxy.GetResult();
    }
}