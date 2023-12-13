using KrMicro.Orders.Models.Enums;
using KrMicro.Orders.Services;

namespace KrMicro.Patterns.Proxy;

public class IncomeChartProxy : IChartAndReport<IncomeChart>
{
    private readonly IOrderService _orderService;
    private IncomeChart? _incomeChart;

    public IncomeChartProxy(TimeRange timeRange, IOrderService orderService)
    {
        TimeRange = timeRange;
        _orderService = orderService;
    }

    public TimeRange TimeRange { get; }

    public async Task<IncomeChart> GetResult()
    {
        if (_incomeChart == null)
            _incomeChart = await new IncomeChart(_orderService)
            {
                TimeRange = TimeRange
            }.GetResult();
        return _incomeChart;
    }
}