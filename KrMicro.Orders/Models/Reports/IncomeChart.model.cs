using KrMicro.Orders.Models.Enums;

namespace KrMicro.Orders.Models.Reports;

public class IncomeChartPoint
{
    public string Label = string.Empty;
    public decimal Total;

    public IncomeChartPoint Clone()
    {
        return new IncomeChartPoint { Total = Total };
    }
}

public class IncomeChart
{
    public List<IncomeChartPoint> Orders = new();
    public List<IncomeChartPoint> Revenue = new();
    public TimeRange TimeRange;
    public List<string> XLabels = new();
}