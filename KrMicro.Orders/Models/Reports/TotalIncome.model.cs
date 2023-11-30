namespace KrMicro.Orders.Models.Reports;

public class TotalIncome
{
    public decimal AveragePerOrder = 0;
    public DateTimeOffset FromDate;
    public decimal Revenue = 0;
    public DateTimeOffset ToDate;
    public int TotalOrders = 0;
    public decimal TotalProducts = 0;
    public decimal TotalSignedCustomer = 0;
    public int TotalSuccessOrders = 0;
}