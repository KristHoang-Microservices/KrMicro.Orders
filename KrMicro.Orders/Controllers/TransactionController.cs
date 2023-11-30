using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.Transaction;
using KrMicro.Orders.CQS.Queries.Transaction;
using KrMicro.Orders.Models;
using KrMicro.Orders.Models.Enums;
using KrMicro.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Orders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService, IOrderService orderService)
    {
        _transactionService = transactionService;
        _orderService = orderService;
    }


    // GET: api/transaction
    [HttpGet]
    public async Task<ActionResult<GetAllTransactionQueryResult>> GetTransactions(
        [FromQuery] GetAllTransactionQueryRequest request)
    {
        var filter = new TransactionQueryFilter(request);

        var list = new List<Transaction>(await _transactionService.GetAllAsync());

        list = list.FindAll(o => filter.Validate(o));
        return new GetAllTransactionQueryResult(new List<Transaction>(list));
    }

    // GET: api/transaction/orderId
    [HttpGet("Orders/{orderId}")]
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

    [HttpPost("Qr")]
    public async Task<ActionResult<CreateTransactionCommandResult>> CreateQrTransaction(
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