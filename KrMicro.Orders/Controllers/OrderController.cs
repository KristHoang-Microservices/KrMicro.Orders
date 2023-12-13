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
    private readonly IPromoService _promoService;
    private readonly ITransactionService _transactionService;

    public OrderController(IOrderService orderService, IOrderDetailService orderDetailService,
        IDeliveryInformationService deliveryInformationService, ITransactionService transactionService,
        IPromoService promoService)
    {
        _orderService = orderService;
        _orderDetailService = orderDetailService;
        _deliveryInformationService = deliveryInformationService;
        _transactionService = transactionService;
        _promoService = promoService;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<ActionResult<GetAllOrderQueryResult>> GetOrders([FromQuery] GetAllOrderQueryRequest request)
    {
        var filter = new OrderQueryFilter(request);

        var list = new List<Order>(await _orderService.GetAllAsync());

        list = list.FindAll(o => filter.Validate(o));

        return Ok(new GetAllOrderQueryResult(list.OrderByDescending(o => o.CreatedAt).ToList()));
    }

    // GET: api/Orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetOrderByIdQueryResult>> GetOrder(short id)
    {
        var item = await _orderService.GetDetailAsync(item => item.Id == id);

        if (item?.Id == null) return BadRequest();

        return new GetOrderByIdQueryResult(item);
    }

    // GET: api/Orders/5
    [HttpGet("{id}/Web")]
    public async Task<ActionResult<GetOrderByIdQueryResult>> GetOrderWeb(short id)
    {
        var item = await _orderService.GetDetailAsync(item => item.Id == id);
        if (item?.Id == null) return BadRequest();
        if (item.Promo != null)
            switch (item.Promo.PromoUnit)
            {
                case PromoUnit.Raw:
                    item.Total -= item.Promo.Value;
                    break;
                case PromoUnit.Percent:
                    item.Total -= item.Promo.Value * item.Total;
                    break;
            }

        return new GetOrderByIdQueryResult(item);
    }

    // PATCH: api/Orders/5
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
        item.Total = 0;
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
                item.Total += newOd.Amount * newOd.Price;
            }
            else
            {
                return Forbid();
            }
        }

        item.Note = request.Note ?? item.Note;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        var order = await _orderService.UpdateAsync(item);
        return new UpdateOrderCommandResult(order);
    }

    // POST: api/Orders
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
            PromoId = request.PromoId,
            Note = request.Note
        };

        var order = await _orderService.InsertAsync(newItem);
        await _orderService.AttachAsync(order);
        foreach (var detail in request.OrderDetails)
        {
            var newDetail = new OrderDetail
            {
                ProductId = detail.ProductId,
                Amount = detail.Amount,
                OrderId = order.Id ?? -1,
                SizeCode = detail.SizeCode,
                CreatedAt = DateTimeOffset.UtcNow
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

                var productStockUpdate = await _client.PostAsync(
                    ProductServiceAPI.UpdateStockById(detail.ProductId, newDetail.SizeCode),
                    JsonContent.Create(new ProductSizes((productSize?.data?.stock ?? 0) - newDetail.Amount)));
                if (!productStockUpdate.IsSuccessStatusCode) return BadRequest();

                order.Total += newDetail.Amount * newDetail.Price;
            }
            else
            {
                return BadRequest();
            }
        }

        order = await _orderService.UpdateAsync(order);
        var deliveryDetail = await _deliveryInformationService.GetDetailAsync(x => x.Id == order.DeliveryInformationId);
        if (request.PaymentMethodId != 3)
        {
            var newTrans = new Transaction
            {
                CustomerId = deliveryDetail?.CustomerId,
                CustomerName = deliveryDetail?.CustomerName,
                PhoneNumber = deliveryDetail?.Phone ?? "",
                OrderId = order.Id ?? -1,
                PaymentMethodId = request.PaymentMethodId,
                TransactionStatus = TransactionStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = Status.Available,
                Total = order.Total
            };

            var promo = await _promoService.GetDetailAsync(p => p.Id == request.PromoId);

            if (promo != null)
                switch (promo.PromoUnit)
                {
                    case PromoUnit.Raw:
                        newTrans.Total -= promo.Value;
                        break;
                    case PromoUnit.Percent:
                        newTrans.Total -= promo.Value * newTrans.Total;
                        break;
                }

            await _transactionService.InsertAsync(newTrans);
        }


        return new CreateOrderCommandResult(order);
    }

    [HttpPost("CheckOrder")]
    public async Task<ActionResult> CheckOrderQuantity(CheckOrderCommandRequest request)
    {
        var outQuantityProducts = new List<FaultProductQuantity>();
        foreach (var detail in request.OrderDetails)
        {
            var productServiceResponse =
                await _client.GetAsync(ProductServiceAPI.GetProductByIdAndSize(detail.ProductId, detail.SizeCode));

            if (productServiceResponse.IsSuccessStatusCode)
            {
                var res = await productServiceResponse.Content.ReadAsStringAsync();
                var productSize = JsonConvert.DeserializeObject<GetByIdQueryResult<ProductSize>>(res);

                if (productSize?.data?.stock < detail.Amount)
                    outQuantityProducts.Add(new FaultProductQuantity(detail.SizeCode, detail.ProductId));
            }
            else
            {
                return BadRequest();
            }
        }

        if (outQuantityProducts.Count > 0)
            return Ok(new CheckOrderCommandResult("Một số sản phẩm đã hết hàng!", outQuantityProducts, false));

        return Ok(new CheckOrderCommandResult("Hàng họ đủ đầy", new List<FaultProductQuantity>()));
    }

    // POST: api/Orders/id
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

    [HttpPost("{id}/Confirmed")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusConfirm(short id)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();

        item.OrderStatus = OrderStatus.Confirmed;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Orders is confirmed!", true);
    }

    [HttpPost("{id}/Delivering")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusDelivering(short id)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();

        item.OrderStatus = OrderStatus.Delivering;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        item.OrderDate = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Orders is on the way!", true);
    }

    [HttpPost("{id}/Returned")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusDelivering(short id,
        DateTimeOffset returnedDate)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();
        if (item.OrderStatus != OrderStatus.Confirmed && item.OrderStatus != OrderStatus.Delivering)
            return new BadRequestObjectResult(new UpdateStatusCommandResult("Orders is not confirmed yet", false));

        item.OrderStatus = OrderStatus.Returned;
        item.OrderDate = returnedDate.ToUniversalTime();
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Orders returned!", true);
    }

    [HttpPost("{id}/Cancelled")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusCancel(short id)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();
        if (item.OrderStatus == OrderStatus.Success || item.OrderStatus == OrderStatus.Returned)
            return new BadRequestObjectResult(new UpdateStatusCommandResult("Orders has been done!", false));

        item.OrderStatus = OrderStatus.Cancelled;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Orders canceled!", true);
    }

    [HttpPost("{id}/Success")]
    public async Task<ActionResult<UpdateStatusCommandResult>> UpdateOrderStatusSuccess(short id,
        DateTimeOffset receiveDate)
    {
        var item = await _orderService.GetDetailAsync(x => x.Id == id);
        if (item?.Id == null) return BadRequest();
        if (item.OrderStatus != OrderStatus.Confirmed && item.OrderStatus != OrderStatus.Delivering)
            return new BadRequestObjectResult(new UpdateStatusCommandResult("Orders is not confirmed yet", false));

        item.OrderStatus = OrderStatus.Success;
        item.OrderDate = receiveDate.ToUniversalTime();
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _orderService.UpdateAsync(item);
        return new UpdateStatusCommandResult("Orders is delivered successfully!", true);
    }

    private async Task<bool> PaymentExists(short id)
    {
        return await _orderService.CheckExistsAsync(e => e.Id == id);
    }
}