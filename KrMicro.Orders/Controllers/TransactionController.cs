using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.Transaction;
using KrMicro.Orders.CQS.Queries.Transaction;
using KrMicro.Orders.Models;
using KrMicro.Orders.Models.Api;
using KrMicro.Orders.Models.Enums;
using KrMicro.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Orders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IPromoService _promoService;
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService, IOrderService orderService,
        IPromoService promoService)
    {
        _transactionService = transactionService;
        _orderService = orderService;
        _promoService = promoService;
    }


    // GET: api/transaction
    [HttpGet]
    public async Task<ActionResult<GetAllTransactionQueryResult>> GetTransactions(
        [FromQuery] GetAllTransactionQueryRequest request)
    {
        var filter = new TransactionQueryFilter(request);

        var list = new List<Transaction>(await _transactionService.GetAllAsync());
        list.Sort((a, b) => b.CreatedAt.Value.CompareTo(a.CreatedAt.Value));
        list = list.FindAll(o => filter.Validate(o));
        return new GetAllTransactionQueryResult(new List<Transaction>(list));
    }

    // GET: api/transaction/orderId
    [HttpGet("Order/{orderId}")]
    public async Task<ActionResult<GetAllTransactionQueryResult>> GetTransactionsByOrderId(short orderId)
    {
        return new GetAllTransactionQueryResult(
            new List<Transaction>(await _transactionService.GetAllWithFilterAsync(x => x.OrderId == orderId)));
    }

    // GET: api/Transaction/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetTransactionByIdQueryResult>> GetTransaction(short id)
    {
        var item = await _transactionService.GetDetailAsync(item => item.Id == id);

        if (item.Id == null) return BadRequest();

        return new GetTransactionByIdQueryResult(item);
    }

    // PATCH: api/Transaction/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateTransactionCommandResult>> PatchTransaction(short id,
        UpdateTransactionCommandRequest request)
    {
        var item = await _transactionService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();

        item.CustomerId = request.CustomerId ?? item.CustomerId;
        item.PhoneNumber = request.PhoneNumber ?? item.PhoneNumber;
        item.OrderId = request.OrderId ?? item.OrderId;
        item.PaymentMethodId = request.PaymentId ?? item.PaymentMethodId;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        var result = await _transactionService.UpdateAsync(item);
        return new UpdateTransactionCommandResult(result);
    }

    // POST: api/Transaction
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("Cash")]
    public async Task<ActionResult<CreateTransactionCommandResult>> CreateCashTransaction(
        CreateTransactionCommandRequest request)
    {
        var order = await _orderService.GetDetailAsync(x => x.Id == request.OrderId);
        if (order == null) return BadRequest("Orders not found");

        var newItem = new Transaction
        {
            OrderId = request.OrderId,
            PaymentMethodId = request.PaymentId,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Available,
            TransactionStatus = TransactionStatus.Pending
        };
        var result = await _transactionService.InsertAsync(newItem);
        return new CreateTransactionCommandResult(result);
    }

    [HttpPost("Banking")]
    public async Task<ActionResult<CreateTransactionCommandResult>> CreateBankingTransaction(
        CreateTransactionCommandRequest request)
    {
        var newItem = new Transaction
        {
            PhoneNumber = request.PhoneNumber,
            OrderId = request.OrderId,
            PaymentMethodId = request.PaymentId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Disable,
            TransactionStatus = TransactionStatus.Pending
        };
        var result = await _transactionService.InsertAsync(newItem);
        return new CreateTransactionCommandResult(result);
    }

    [HttpPost("VnPay")]
    public async Task<ActionResult<CreateVnPayTransactionCommandResult>> CreateVnPayTransaction(
        CreateVnPayTransactionCommandRequest request)
    {
        var order = await _orderService.GetDetailAsync(x => x.Id == request.OrderId);
        var transaction = new Transaction
        {
            PhoneNumber = request.PhoneNumber,
            OrderId = request.OrderId,
            PaymentMethodId = request.PaymentMethodId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Disable,
            TransactionStatus = TransactionStatus.Pending,
            Total = order?.Total ?? 0
        };

        if (order?.Promo != null)
            switch (order?.Promo.PromoUnit)
            {
                case PromoUnit.Raw:
                    transaction.Total -= order.Promo.Value;
                    break;
                case PromoUnit.Percent:
                    transaction.Total -= order.Promo.Value * transaction.Total;
                    break;
            }

        transaction = await _transactionService.InsertAsync(transaction);

        var vnpay = new VnPayLibrary();

        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", "HVR0T9T5");
        vnpay.AddRequestData("vnp_Amount", $"{(int)(transaction.Total * 100)}");

        // Can be add other type of billing

        vnpay.AddRequestData("vnp_CreateDate",
            DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang : " + transaction.OrderId);
        vnpay.AddRequestData("vnp_OrderType", "other");

        vnpay.AddRequestData("vnp_ReturnUrl", "https://localhost:7140/api/Transaction/VnPay/VnPayReturn");
        vnpay.AddRequestData("vnp_TxnRef", $"{transaction.Id}");

        var paymentUrl = vnpay.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
            "BLWIVSEWVFLOGDKXPXNGNHQUZTBDTHEZ");

        return new CreateVnPayTransactionCommandResult(paymentUrl, transaction.Id ?? -1);
    }

    [HttpGet("VnPay/VnPayReturn")]
    public async Task<ActionResult> VnPayReturnMessage()
    {
        var vnpayData = HttpContext.Request.Query;
        var vnpay = new VnPayLibrary();
        var vnp_HashSecret = "BLWIVSEWVFLOGDKXPXNGNHQUZTBDTHEZ";
        foreach (var s in vnpayData)
            //get all querystring data
            if (!string.IsNullOrEmpty(s.Value) && s.Key.StartsWith("vnp_"))
                vnpay.AddResponseData(s.Key, s.Value);

        var transactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
        var vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
        var vnpResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        var vnpTransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
        string vnpSecureHash = HttpContext.Request.Query["vnp_SecureHash"];


        var checkSignature = vnpay.ValidateSignature(vnpSecureHash, vnp_HashSecret);
        if (checkSignature)
        {
            var item = await _transactionService.GetDetailAsync(x => x.Id == transactionId);
            if (item == null) return BadRequest();
            if (vnpResponseCode == "00" && vnpTransactionStatus == "00")
            {
                //Thanh toan thanh cong
                item.TransactionStatus = TransactionStatus.Success;
                item.UpdatedAt = DateTimeOffset.UtcNow;
                await _transactionService.UpdateAsync(item);
                return Redirect(ClientSideHost.GetReturnUrl(item.OrderId));
            }

            item.TransactionStatus = TransactionStatus.Failed;
            item.UpdatedAt = DateTimeOffset.UtcNow;
            //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
            return BadRequest("Đã xảy ra lỗi giao dịch!");
        }

        return BadRequest("Lỗi giao dịch");
    }

    [HttpGet("CheckVnPay/{transactionId}")]
    public async Task<ActionResult> CheckVnPay(short transactionId)
    {
        var transaction = await _transactionService.GetDetailAsync(x => x.Id == transactionId);
        if (transaction == null) return BadRequest("Not Found");

        while (transaction.TransactionStatus != TransactionStatus.Success) return BadRequest("Pending");
        return Ok(transactionId);
    }

    // POST: api/Transaction/id
    [HttpPost("{id}/Success")]
    public async Task<ActionResult<UpdateTransactionStatusCommandResult>> ConfirmPaid(short id)
    {
        var item = await _transactionService.GetDetailAsync(x => x.Id == id);
        if (item == null) return BadRequest();
        item.TransactionStatus = TransactionStatus.Success;
        item.CreatedAt = DateTimeOffset.UtcNow;
        await _transactionService.UpdateAsync(item);

        return new UpdateTransactionStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }

    // POST: api/Transaction/id
    [HttpPost("{id}/Cancel")]
    public async Task<ActionResult<UpdateTransactionStatusCommandResult>> CancelTransaction(short id)
    {
        var item = await _transactionService.GetDetailAsync(x => x.Id == id);
        if (item == null) return BadRequest();
        item.TransactionStatus = TransactionStatus.Cancel;
        item.CreatedAt = DateTimeOffset.UtcNow;
        await _transactionService.UpdateAsync(item);

        return new UpdateTransactionStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }

    // POST: api/Transaction/id
    [HttpPost("{id}/Failed")]
    public async Task<ActionResult<UpdateTransactionStatusCommandResult>> SetFailedTransaction(short id)
    {
        var item = await _transactionService.GetDetailAsync(x => x.Id == id);
        if (item == null) return BadRequest();
        item.TransactionStatus = TransactionStatus.Failed;
        item.CreatedAt = DateTimeOffset.UtcNow;
        await _transactionService.UpdateAsync(item);

        return new UpdateTransactionStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }

    // POST: api/Transaction/id
    [HttpPost("{id}/UpdateStatus")]
    public async Task<ActionResult<UpdateTransactionStatusCommandResult>> UpdateStatus(short id,
        UpdateTransactionStatusRequest request)
    {
        var item = await _transactionService.GetDetailAsync(x => x.Id == id);
        if (item.Id == null) return BadRequest();
        item.Status = request.Status;
        item.CreatedAt = DateTimeOffset.UtcNow;
        await _transactionService.UpdateAsync(item);

        return new UpdateTransactionStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }

    private async Task<bool> TransactionExists(short id)
    {
        return await _transactionService.CheckExistsAsync(e => e.Id == id);
    }
}