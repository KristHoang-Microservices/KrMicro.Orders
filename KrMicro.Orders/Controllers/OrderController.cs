using KrMicro.Core.CQS.Query.Abstraction;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.Order;
using KrMicro.Orders.CQS.Commands.Payment;
using KrMicro.Orders.CQS.Queries.Payment;
using KrMicro.Orders.Models;
using KrMicro.Orders.Models.Enums;
using KrMicro.Orders.Models.ProductServices;
using KrMicro.Orders.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KrMicro.Orders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly HttpClient _client = new();
    private readonly IDeliveryInformationService _deliveryInformationService;
    private readonly IOrderDetailService _orderDetailService;
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService, IOrderDetailService orderDetailService,
        IDeliveryInformationService deliveryInformationService)
    {
        _orderService = orderService;
        _orderDetailService = orderDetailService;
        _deliveryInformationService = deliveryInformationService;
    }

    // GET: api/Order
    [HttpGet]
    public async Task<ActionResult<GetAllOrderQueryResult>> GetOrder()
    {
        return new GetAllOrderQueryResult(new List<Order>(await _orderService.GetAllAsync()));
    }

    // GET: api/Order/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetOrderByIdQueryResult>> GetOrder(short id)
    {
        var item = await _orderService.GetDetailAsync(item => item.Id == id);

        if (item?.Id == null) return BadRequest();

        return new GetOrderByIdQueryResult(item);
    }

    // PATCH: api/Order/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateOrderCommandResult>> PatchOrder(short id, UpdateOrderCommandRequest request)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();
        if (request.DeliveryInformationId != null)
        {
            var deliveryInfor =
                await _deliveryInformationService.GetDetailAsync(dl => dl.Id == request.DeliveryInformationId);
            item.DeliveryInformationId = deliveryInfor?.Id ?? item.DeliveryInformationId;

            item = await _orderService.UpdateAsync(item);
        }

        var updateTask = new List<Task>();
        foreach (var itemInOrder in item.OrderDetails)
            updateTask.Add(Task.Run(() => _orderDetailService.DeleteAsync(itemInOrder)));

        foreach (var t in updateTask) await t;
        item.OrderDetails = new List<OrderDetail>();
        item = await _orderService.UpdateAsync(item);
        foreach (var od in request.OrderDetails)
        {
            var newOd = new OrderDetail
            {
                SizeCode = od.SizeCode,
                Amount = od.Amount,
                ProductId = od.ProductId,
                OrderId = id
            };
            var productServiceResponse =
                await _client.GetAsync(ProductServiceAPI.GetProductByIdAndSize(od.ProductId, od.SizeCode));

            if (productServiceResponse.IsSuccessStatusCode)
            {
                var res = await productServiceResponse.Content.ReadAsStringAsync();
                var productSize = JsonConvert.DeserializeObject<GetByIdQueryResult<ProductSize>>(res);

                newOd.Price = productSize?.data?.price ?? 0;
                newOd.SizeId = productSize?.data?.sizeId ?? 0;
                await _orderDetailService.InsertAsync(newOd);
            }
            else
            {
                return Forbid();
            }
        }

        item.Note = request.Note ?? item.Note;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        var result = await _orderService.UpdateAsync(item);
        return new UpdateOrderCommandResult(result);
    }

    // POST: api/Order
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CreateOrderCommandResult>> CreateOrder(CreateOrderCommandRequest request)
    {
        var newItem = new Order
        {
            DeliveryInformationId = request.DeliveryInformationId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Available,
            OrderStatus = OrderStatus.Pending,
            Note = request.Note
        };
        var result = await _orderService.InsertAsync(newItem);
        await _orderService.AttachAsync(result);

        foreach (var detail in request.OrderDetails)
        {
            var newDetail = new OrderDetail
            {
                ProductId = detail.ProductId,
                Amount = detail.Amount,
                OrderId = result.Id ?? -1,
                SizeCode = detail.SizeCode
            };

            var productServiceResponse =
                await _client.GetAsync(ProductServiceAPI.GetProductByIdAndSize(detail.ProductId, detail.SizeCode));

            if (productServiceResponse.IsSuccessStatusCode)
            {
                var res = await productServiceResponse.Content.ReadAsStringAsync();
                var productSize = JsonConvert.DeserializeObject<GetByIdQueryResult<ProductSize>>(res);

                newDetail.Price = productSize?.data?.price ?? 0;
                newDetail.SizeId = productSize?.data?.sizeId ?? 0;
                await _orderDetailService.InsertAsync(newDetail);
            }
            else
            {
                return BadRequest();
            }
        }

        return new CreateOrderCommandResult(result);
    }

    // POST: api/Order/id
    [HttpPost("{id}/UpdateStatus")]
    public async Task<ActionResult<UpdateOrderStatusCommandResult>> UpdateStatus(short id,
        UpdatePaymentStatusRequest request)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();
        item.Status = request.Status;
        item.CreatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);

        return new UpdateOrderStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }

    // POST: api/Order/id/Confirm

    private async Task<bool> PaymentExists(short id)
    {
        return await _orderService.CheckExistsAsync(e => e.Id == id);
    }
}