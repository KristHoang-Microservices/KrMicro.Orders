using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.Payment;
using KrMicro.Orders.CQS.Queries.Payment;
using KrMicro.Orders.Models;
using KrMicro.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Orders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // GET: api/PaymentMethod
    [HttpGet]
    public async Task<ActionResult<GetAllPaymentQueryResult>> GetPayment()
    {
        return new GetAllPaymentQueryResult(new List<PaymentMethod>(await _paymentService.GetAllAsync()));
    }

    // GET: api/PaymentMethod/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPaymentByIdQueryResult>> GetPayment(short id)
    {
        var item = await _paymentService.GetDetailAsync(item => item.Id == id);

        if (item == null) return BadRequest();

        return new GetPaymentByIdQueryResult(item);
    }

    // PATCH: api/PaymentMethod/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdatePaymentCommandResult>> PatchPayment(short id,
        UpdatePaymentCommandRequest request)
    {
        var item = await _paymentService.GetDetailAsync(x => x.Id == id);
        if (item.Id == null) return BadRequest();
        item.Name = request.Name ?? item.Name;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        var result = await _paymentService.UpdateAsync(item);
        return new UpdatePaymentCommandResult(result);
    }

    // POST: api/PaymentMethod
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CreatePaymentCommandResult>> CreatePayment(CreatePaymentCommandRequest request)
    {
        var newItem = new PaymentMethod
        {
            Name = request.Name,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Disable
        };
        var result = await _paymentService.InsertAsync(newItem);
        return new CreatePaymentCommandResult(result);
    }

    // POST: api/PaymentMethod/id
    [HttpPost("{id}/UpdateStatus")]
    public async Task<ActionResult<UpdatePaymentStatusCommandResult>> UpdateStatus(short id,
        UpdatePaymentStatusRequest request)
    {
        var item = await _paymentService.GetDetailAsync(x => x.Id == id);
        if (item.Id == null) return BadRequest();
        item.Status = request.Status;
        item.CreatedAt = DateTimeOffset.UtcNow;
        await _paymentService.UpdateAsync(item);

        return new UpdatePaymentStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }

    private async Task<bool> PaymentExists(short id)
    {
        return await _paymentService.CheckExistsAsync(e => e.Id == id);
    }
}