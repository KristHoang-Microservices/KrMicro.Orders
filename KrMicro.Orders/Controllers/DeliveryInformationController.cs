using System.Text.Json;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.DeliveryInformation;
using KrMicro.Orders.CQS.Commands.Payment;
using KrMicro.Orders.CQS.Queries.Api.Customer;
using KrMicro.Orders.CQS.Queries.DeliveryInformation;
using KrMicro.Orders.CQS.Queries.Payment;
using KrMicro.Orders.Models;
using KrMicro.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Orders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveryInformationController : ControllerBase
{
    private readonly IDeliveryInformationService _deliveryInformationService;

    public DeliveryInformationController(IDeliveryInformationService deliveryInformationService)
    {
        _deliveryInformationService = deliveryInformationService;
    }

    // GET: api/DeliveryInformation
    [HttpGet]
    public async Task<ActionResult<GetAllDeliveryInformationQueryResult>> GetAllDeliveryInformation()
    {
        return new GetAllDeliveryInformationQueryResult(
            new List<DeliveryInformation>(await _deliveryInformationService.GetAllAsync()));
    }

    // GET: api/DeliveryInformation/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetDeliveryInformationByIdQueryResult>> GetDeliveryInformation(short id)
    {
        var item = await _deliveryInformationService.GetDetailAsync(item => item.Id == id);

        if (item == null) return BadRequest();

        return new GetDeliveryInformationByIdQueryResult(item);
    }

    // PATCH: api/DeliveryInformation/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateDeliveryInformationCommandResult>> PatchDeliveryInformation(short id,
        UpdateDeliveryInformationCommandRequest request)
    {
        var item = await _deliveryInformationService.GetDetailAsync(x => x.Id == id);
        if (item == null) return BadRequest();

        item.Name = request.Name ?? item.Name;
        item.FullAddress = request.FullAddress ?? item.FullAddress;
        item.CustomerName = request.CustomerName ?? item.CustomerName;
        item.Phone = request.Phone ?? item.Phone;
        item.UpdatedAt = DateTimeOffset.UtcNow;

        var result = await _deliveryInformationService.UpdateAsync(item);
        return new UpdateDeliveryInformationCommandResult(result);
    }

    // POST: api/DeliveryInformation
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CreateDeliveryInformationCommandResult>> CreateDeliveryInformation(
        CreateDeliveryInformationCommandRequest request)
    {
        if (request.FullAddress == string.Empty) return BadRequest("The delivery address is missing");

        var newItem = new DeliveryInformation
        {
            Name = request.Name,
            CustomerName = request.CustomerName,
            Phone = request.Phone,
            FullAddress = request.FullAddress,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Available
        };

        if (request.CustomerId != null)
        {
            var client = await ApiHttpClient.GetHttpClient();

            var res = await client.GetAsync(CustomerServiceAPI.GetCustomerById(request.CustomerId ?? -1));
            if (res.IsSuccessStatusCode)
            {
                var customer =
                    JsonSerializer.Deserialize<GetCustomerDetailQueryResult>(await res.Content.ReadAsStringAsync());
                newItem.Phone = customer?.phone ?? newItem.Phone;
                newItem.CustomerName = customer?.name ?? newItem.CustomerName;
                newItem.CustomerId = request.CustomerId;
            }
            else
            {
                return BadRequest("Customer Id wasn't correct mapped");
            }
        }

        var result = await _deliveryInformationService.InsertAsync(newItem);
        return new CreateDeliveryInformationCommandResult(result);
    }

    // POST: api/DeliveryInformation/id
    [HttpPost("{id}/UpdateStatus")]
    public async Task<ActionResult<UpdateDeliveryInformationStatusCommandResult>> UpdateStatus(short id,
        UpdateDeliveryInformationStatusRequest request)
    {
        var item = await _deliveryInformationService.GetDetailAsync(x => x.Id == id);
        if (item == null) return BadRequest();
        item.Status = request.Status;
        item.CreatedAt = DateTimeOffset.UtcNow;
        await _deliveryInformationService.UpdateAsync(item);

        return new UpdateDeliveryInformationStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }

    private async Task<bool> DeliveryInformationExists(short id)
    {
        return await _deliveryInformationService.CheckExistsAsync(e => e.Id == id);
    }
}