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

public class DeliveryInformationController : ControllerBase
{
    private IDeliveryInformationService _deliveryinformationService;

    public DeliveryInformationController(IDeliveryInformationService deliveryInformationService)
    {
        _deliveryinformationService = deliveryInformationService;
    }

    // GET: api/DeliveryInformation
    [HttpGet]
    public async Task<ActionResult<GetAllDeliveryInformationQueryResult>> GetDeliveryInformation()
    {
        return new GetAllDeliveryInformationQueryResult(new List<DeliveryInformation>(await _deliveryinformationService.GetAllAsync()));
    }
    
    // GET: api/DeliveryInformation/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetDeliveryInformationByIdQueryResult>> GetDeliveryInformation(short id)
    {
        var item = await _deliveryinformationService.GetDetailAsync(item => item.Id == id);

        if (item.Id == null) return BadRequest();

        return new GetDeliveryInformationByIdQueryResult(item);
    }
    
    // PATCH: api/DeliveryInformation/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateDeliveryInformationCommandResult>> PatchDeliveryInformation(short id, UpdateDeliveryInformationCommandRequest request)
    {
        var item = await _deliveryinformationService.GetDetailAsync(x => x.Id == id);
        if (item.Id == null) return BadRequest();
        item.Name = request.Name ?? item.Name;
        item.FullAddress = request.FullAddress ?? item.FullAddress;
        item.Phone = request.Phone ?? item.Phone;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        var result = await _deliveryinformationService.UpdateAsync(item);
        return new UpdateDeliveryInformationCommandResult(result);
    }
    
    // POST: api/DeliveryInformation
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CreateDeliveryInformationCommandResult>> CreateDeliveryInformation(CreateDeliveryInformationCommandRequest request)
    {
        var newItem = new DeliveryInformation
        {
            Name = request.Name,
            Phone = request.Phone,
            FullAddress = request.FullAddress,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Disable
        };
        var result = await _deliveryinformationService.InsertAsync(newItem);
        return new CreateDeliveryInformationCommandResult(result);
    }
    
    // POST: api/DeliveryInformation/id
    [HttpPost("{id}/UpdateStatus")]
    public async Task<ActionResult<UpdateDeliveryInformationStatusCommandResult>> UpdateStatus(short id,
        UpdateDeliveryInformationStatusRequest request)
    {
        var item = await _deliveryinformationService.GetDetailAsync(x => x.Id == id);
        if (item.Id == null) return BadRequest();
        item.Status = request.Status;
        item.CreatedAt = DateTimeOffset.UtcNow;
        await _deliveryinformationService.UpdateAsync(item);

        return new UpdateDeliveryInformationStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }
    
    private async Task<bool> DeliveryInformationExists(short id)
    {
        return await _deliveryinformationService.CheckExistsAsync(e => e.Id == id);
    }
}