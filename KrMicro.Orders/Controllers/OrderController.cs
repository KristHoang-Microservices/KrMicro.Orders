using KrMicro.Core.CQS.Query.Abstraction;
using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.Payment;
using KrMicro.Orders.CQS.Queries.Payment;
using KrMicro.Orders.Models;
using KrMicro.Orders.Models.Enums;
using KrMicro.Orders.Models.ProductServices;
using KrMicro.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Orders.Controllers;

[Route("api/[controller]")]
[ApiController]

public class OrderController : ControllerBase
{
    private IOrderService _orderService;
    private IOrderDetailService _orderDetailService;

    private HttpClient client = new();

    public OrderController(IOrderService orderService, IOrderDetailService orderDetailService)
    {
        _orderService = orderService;
        _orderDetailService = orderDetailService;
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

        if (item.Id == null) return BadRequest();

        return new GetOrderByIdQueryResult(item);
    }
    
    // PATCH: api/Order/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateOrderCommandResult>> PatchOrder(short id, UpdateOrderCommandRequest request)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item.Id == null) return BadRequest();
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
            OrderStatus = OrderStatus.Pending
        };
        var result = await _orderService.InsertAsync(newItem);

        foreach (var detail in request.OrderDetails)
        {
            var newDetail = new OrderDetail
            {
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                Order = result,
                SizeCode = detail.SizeCode
            };

            HttpResponseMessage productServiceResponse =
                await client.GetAsync(ProductServiceAPI.GetProductByIdAndSize(detail.ProductId, detail.SizeCode));

            if (productServiceResponse.IsSuccessStatusCode)
            {
                newDetail.Price =
                    (await productServiceResponse.Content.ReadFromJsonAsync<GetByIdQueryResult<ProductPrice>>()).Data.price;    
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
        if (item.Id == null) return BadRequest();
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