using KrMicro.Orders.CQS.Queries.Order;
using KrMicro.Orders.Models;
using KrMicro.Orders.Models.Enums;
using KrMicro.Orders.Models.Reports;
using KrMicro.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Orders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ITransactionService _transactionService;

    public ReportController(ITransactionService transactionService, IOrderService orderService)
    {
        _transactionService = transactionService;
        _orderService = orderService;
    }

    [HttpGet("TotalIncome")]
    public async Task<ActionResult> GetIncomeReport([FromQuery] DateTimeOffset FromDate,
        [FromQuery] DateTimeOffset ToDate)
    {
        var report = new TotalIncome
        {
            FromDate = FromDate,
            ToDate = ToDate
        };
        var filter = new OrderQueryFilter(new GetAllOrderQueryRequest(null, null, FromDate, ToDate));
        var orders = new List<Order>(await _orderService.GetAllAsync()).FindAll(o => filter.Validate(o));

        orders.ForEach(o =>
        {
            if (o.OrderStatus != OrderStatus.Success) return;
            if (o.Transactions.FirstOrDefault(t => t.TransactionStatus == TransactionStatus.Success) == null) return;

            report.TotalProducts += o.OrderDetails.Count;
            report.Revenue += o.Total;
            report.TotalSuccessOrders += 1;
        });
        report.AveragePerOrder = report.TotalSuccessOrders != 0 ? report.Revenue / report.TotalSuccessOrders : 0;
        report.TotalOrders = orders.Count;

        return Ok(report);
    }

    [HttpGet("IncomeChart")]
    public async Task<ActionResult> GetIncomeChart([FromQuery] TimeRange timeRange)
    {
        var chart = new IncomeChart
        {
            TimeRange = timeRange
        };

        var toDate = DateTimeOffset.Now;
        var fromDate = toDate;
        switch (timeRange)
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
        chart.Revenue = new List<IncomeChartPoint>
            { init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone() };
        chart.Orders = new List<IncomeChartPoint>
            { init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone(), init.Clone() };
        chart.XLabels = new List<string> { "", "", "", "", "", "", "" };

        var startAt = new DateTimeOffset(fromDate.Date);
        for (var i = 0; i < 7; i++)
            switch (timeRange)
            {
                case TimeRange.Day:
                    var start = startAt.Date;
                    chart.XLabels[i] = startAt.LocalDateTime.ToShortDateString();
                    var end = start.Date.AddDays(1).AddSeconds(-1);

                    var filter = new OrderQueryFilter(new GetAllOrderQueryRequest(null, null, start, end));
                    var filteredOrders = orders.FindAll(o => filter.Validate(o));
                    foreach (var order in filteredOrders)
                        chart.Revenue[i].Total += order.Total;

                    chart.Revenue[i].Label = chart.XLabels[i];
                    chart.Orders[i].Total = filteredOrders.Count;
                    startAt = startAt.AddDays(1);
                    break;
                case TimeRange.Month:
                    start = new DateTime(startAt.Year, startAt.Month, 1);
                    chart.XLabels[i] = "Tháng " + startAt.Month + ", " + startAt.Year;

                    end = start.Date.AddMonths(1).AddSeconds(-1);

                    filter = new OrderQueryFilter(new GetAllOrderQueryRequest(null, null, start, end));
                    filteredOrders = orders.FindAll(o => filter.Validate(o));
                    foreach (var order in filteredOrders)
                        chart.Revenue[i].Total += order.Total;

                    chart.Revenue[i].Label = chart.XLabels[i];
                    chart.Orders[i].Total = filteredOrders.Count;
                    startAt = startAt.AddMonths(1);
                    break;
                case TimeRange.Year:
                    start = new DateTime(startAt.Year, 1, 1);
                    chart.XLabels[i] = "Năm " + startAt.LocalDateTime.ToString("yyyy");
                    end = start.Date.AddYears(1).AddSeconds(-1);

                    filter = new OrderQueryFilter(new GetAllOrderQueryRequest(null, null, start, end));
                    filteredOrders = orders.FindAll(o => filter.Validate(o));
                    foreach (var order in filteredOrders)
                        chart.Revenue[i].Total += order.Total;

                    chart.Revenue[i].Label = chart.XLabels[i];
                    chart.Orders[i].Total = filteredOrders.Count;
                    startAt = startAt.AddYears(1);
                    break;
            }

        return Ok(chart);
    }
}