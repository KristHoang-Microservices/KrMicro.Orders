using KrMicro.Orders.Models.Api;
using Microsoft.AspNetCore.Http;

namespace KrMicro.Patterns.Factory;

public class VnPayTransaction : AbstractTransaction
{
    public string GetVnPayLink(HttpContext httpContext)
    {
        var vnpay = new VnPayLibrary();
        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", "HVR0T9T5");
        vnpay.AddRequestData("vnp_Amount", $"{(int)(Total * 100)}");

        // Can be add other type of billing

        vnpay.AddRequestData("vnp_CreateDate",
            DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang : " + OrderId);
        vnpay.AddRequestData("vnp_OrderType", "other");

        vnpay.AddRequestData("vnp_ReturnUrl", "https://localhost:7140/api/AbstractTransaction/VnPay/VnPayReturn");
        vnpay.AddRequestData("vnp_TxnRef", $"{Id}");

        var paymentUrl = vnpay.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
            "BLWIVSEWVFLOGDKXPXNGNHQUZTBDTHEZ");

        return paymentUrl;
    }
}