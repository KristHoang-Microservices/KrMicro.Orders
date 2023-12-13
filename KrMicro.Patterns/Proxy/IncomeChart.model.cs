using KrMicro.Orders.CQS.Queries.Order;
using KrMicro.Orders.Models;
using KrMicro.Orders.Models.Enums;
using KrMicro.Orders.Services;

namespace KrMicro.Patterns.Proxy;

public class IncomeChartPoint
{
    public string Label = string.Empty;
    public decimal Total;

    public IncomeChartPoint Clone()
    {
        return new IncomeChartPoint { Total = Total };
    }
}

public class IncomeChart : IChartAndReport<IncomeChart>
{
    private readonly IOrderService _orderService;
    private List<IncomeChartPoint> _orders = new();
    private List<IncomeChartPoint> _revenue = new();
    private List<string> _xLabels = new();
    public TimeRange TimeRange;

    public IncomeChart(IOrderService orderService)
    {
        _orderService = orderService;
    }


    public async Task<IncomeChart> GetResult()
    {
        var toDate = DateTimeOffset.Now;
        var fromDate = toDate;
        switch (TimeRange)
        {
            case TimeRange.Day:
                fromDate = fromDate.AddDays(-6);
                break;
            case TimeRange.Month:
                fromDate = fromDate.AddMonths(-6);
                break;
            case TimeRange.Year:
                fromDate = fromDate.AddYears(-6);
                break;
        }


        var orders = new List<Order>(await _orderService.GetAllAsync()).FindAll(o =>
            new OrderQueryFilter(new GetAllOrderQueryRequest(OrderStatus.Success, null, fromDate, toDate)).Validate(o));

        var init = new IncomeChartPoint { Total = 0 };
        _revenue = new List<IncomeChartPoint>
            { init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone() };
        _orders = new List<IncomeChartPoint>
            { init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone() };
        _xLabels = new List<string> { "", "", "", "", "", "", "" };

        var startAt = new DateTimeOffset(fromDate.Date);
        for (var i = 0; i < 7; i++)
            switch (TimeRange)
            {
                case TimeRange.Day:
                    var start = startAt.Date;
                    _xLabels[i] = startAt.LocalDateTime.ToShortDateString();
                    var end = start.Date.AddDays(1).AddSeconds(-1);

                    var filter = new OrderQueryFilter(new GetAllOrderQueryRequest(null, null, start, end));
                    var filteredOrders = orders.FindAll(o => filter.Validate(o));
                    foreach (var order in filteredOrders)
                        _revenue[i].Total += order.Total;

                    _revenue[i].Label = _xLabels[i];
                    _orders[i].Total = filteredOrders.Count;
                    startAt = startAt.AddDays(1);
                    break;
                case TimeRange.Month:
                    start = new DateTime(startAt.Year, startAt.Month, 1);
                    _xLabels[i] = "Tháng " + startAt.Month + ", " + startAt.Year;

                    end = start.Date.AddMonths(1).AddSeconds(-1);

                    filter = new OrderQueryFilter(new GetAllOrderQueryRequest(null, null, start, end));
                    filteredOrders = orders.FindAll(o => filter.Validate(o));
                    foreach (var order in filteredOrders)
                        _revenue[i].Total += order.Total;

                    _revenue[i].Label = _xLabels[i];
                    _orders[i].Total = filteredOrders.Count;
                    startAt = startAt.AddMonths(1);
                    break;
                case TimeRange.Year:
                    start = new DateTime(startAt.Year, 1, 1);
                    _xLabels[i] = "Năm " + startAt.LocalDateTime.ToString("yyyy");
                    end = start.Date.AddYears(1).AddSeconds(-1);

                    filter = new OrderQueryFilter(new GetAllOrderQueryRequest(null, null, start, end));
                    filteredOrders = orders.FindAll(o => filter.Validate(o));
                    foreach (var order in filteredOrders)
                        _revenue[i].Total += order.Total;

                    _revenue[i].Label = _xLabels[i];
                    _orders[i].Total = filteredOrders.Count;
                    startAt = startAt.AddYears(1);
                    break;
            }

        return this;
    }
}