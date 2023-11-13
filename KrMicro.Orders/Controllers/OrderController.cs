using KrMicro.Core.CQS.Command.Abstraction;
using KrMicro.Core.CQS.Query.Abstraction;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.Order;
using KrMicro.Orders.CQS.Commands.Payment;
using KrMicro.Orders.CQS.Queries.Order;
using KrMicro.Orders.Models;
using KrMicro.Orders.Models.Api.Products;
using KrMicro.Orders.Models.Enums;
using KrMicro.Orders.Models.ProductServices;
using KrMicro.Orders.Services;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<GetAllOrderQueryResult>> GetOrders()
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
    [Authorize]
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
            var productSizes = new List<ProductSizes>();
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

                productSizes.Add(new ProductSizes
                {
                    sizeCode = newDetail.SizeCode,
                    stock = (productSize?.data?.stock ?? 0) - newDetail.Amount
                });
                var productStockUpdate = await _client.PostAsync(ProductServiceAPI.UpdateStockById(detail.ProductId),
                    JsonContent.Create(productSizes));

                if (productStockUpdate.IsSuccessStatusCode) return BadRequest();
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
    [Authorize("Admin,Employee,Customer")]
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

    [HttpPost("{id}/Confirm")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusConfirm(short id)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();

        item.OrderStatus = OrderStatus.Confirmed;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Order is confirmed!", true);
    }

    [HttpPost("{id}/Delivering")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusDelivering(short id)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();

        item.OrderStatus = OrderStatus.Delivering;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Order is on the way!", true);
    }

    [HttpPost("{id}/Returned")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusDelivering(short id,
        DateTimeOffset returnedDate)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();
        if (item.OrderStatus != OrderStatus.Confirmed && item.OrderStatus != OrderStatus.Delivering)
            return new BadRequestObjectResult(new UpdateStatusCommandResult("Order is not confirmed yet", false));

        item.OrderStatus = OrderStatus.Returned;
        item.OrderDate = returnedDate.ToUniversalTime();
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Order returned!", true);
    }

    [HttpPost("{id}/Canceled")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusCancel(short id)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();
        if (item.OrderStatus == OrderStatus.Success || item.OrderStatus == OrderStatus.Returned)
            return new BadRequestObjectResult(new UpdateStatusCommandResult("Order has been done!", false));

        item.OrderStatus = OrderStatus.Cancelled;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Order canceled!", true);
    }

    [HttpPost("{id}/Success")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusSuccess(short id,
        DateTimeOffset receiveDate)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();
        if (item.OrderStatus != OrderStatus.Confirmed && item.OrderStatus != OrderStatus.Delivering)
            return new BadRequestObjectResult(new UpdateStatusCommandResult("Order is not confirmed yet", false));

        item.OrderStatus = OrderStatus.Cancelled;
        item.OrderDate = receiveDate.ToUniversalTime();
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Order is delivered successfully!", true);
    }

    private async Task<bool> PaymentExists(short id)
    {
        return await _orderService.CheckExistsAsync(e => e.Id == id);
    }
}