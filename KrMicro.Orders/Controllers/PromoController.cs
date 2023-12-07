using KrMicro.Core.Models.Abstraction;
using KrMicro.Orders.Constants;
using KrMicro.Orders.CQS.Commands.Promo;
using KrMicro.Orders.CQS.Queries.Promo;
using KrMicro.Orders.Models;
using KrMicro.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Orders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PromoController : ControllerBase
{
    private readonly IPromoService _promoService;

    public PromoController(IPromoService promoService)
    {
        _promoService = promoService;
    }


    // GET: api/
    [HttpGet]
    public async Task<ActionResult> GetPromos()
    {
        return Ok(new GetAllPromoQueryResult(new List<Promo>(await _promoService.GetAllAsync())));
    }

    // GET: api/Promo/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPromoByIdQueryResult>> GetPromo(short id)
    {
        var item = await _promoService.GetDetailAsync(item => item.Id == id);

        if (item == null) return BadRequest();

        return new GetPromoByIdQueryResult(item);
    }

    // GET: api/Promo/Code/5
    [HttpGet("Code/{code}")]
    public async Task<ActionResult<GetPromoByIdQueryResult>> GetPromo(string code)
    {
        var item = await _promoService.GetDetailAsync(item => item.Code == code);

        if (item == null) return BadRequest();

        return new GetPromoByIdQueryResult(item);
    }

    // PATCH: api/Promo/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdatePromoCommandResult>> PatchPromo(short id,
        UpdatePromoCommandRequest request)
    {
        var item = await _promoService.GetDetailAsync(x => x.Id == id);
        if (item == null) return BadRequest();
        if (request.Value.HasValue & (request.Value <= 0)) return BadRequest();
        if (request is { StartDate: not null, EndDate: not null } && request.StartDate.HasValue &
            request.EndDate.HasValue & (request.EndDate.Value.CompareTo(request.StartDate.Value) < 0))
            return BadRequest();
        item.Name = request.Name ?? item.Name;
        item.Code = request.Code ?? item.Code;
        item.Value = request.Value ?? item.Value;
        item.PromoUnit = request.PromoUnit ?? item.PromoUnit;
        item.StartDate = request.StartDate ?? item.StartDate;
        item.EndDate = request.EndDate ?? item.EndDate;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        var result = await _promoService.UpdateAsync(item);
        return new UpdatePromoCommandResult(result);
    }

    // POST: api/Promo
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CreatePromoCommandResult>> CreatePromo(CreatePromoCommandRequest request)
    {
        if (request.Value <= 0) return BadRequest();
        if (request.EndDate.CompareTo(request.StartDate) < 0) return BadRequest();
        var newItem = new Promo
        {
            Name = request.Name,
            Code = request.Code,
            Value = request.Value,
            PromoUnit = request.PromoUnit,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Available
        };
        var result = await _promoService.InsertAsync(newItem);
        return new CreatePromoCommandResult(result);
    }

    // POST: api/Promo/id
    [HttpPost("{id}/UpdateStatus")]
    public async Task<ActionResult<UpdatePromoStatusCommandResult>> UpdateStatus(short id,
        UpdatePromoStatusRequest request)
    {
        var item = await _promoService.GetDetailAsync(x => x.Id == id);
        if (item == null) return BadRequest();
        item.Status = request.Status;
        item.CreatedAt = DateTimeOffset.UtcNow;
        await _promoService.UpdateAsync(item);

        return new UpdatePromoStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }

    // POST: api/Promo/CheckPromo
    [HttpGet("{code}/CheckPromo")]
    public async Task<bool> CheckPromo(string code)
    {
        var promo = await _promoService.GetDetailAsync(e => e.Code == code && e.Status == Status.Available);
        if (promo == null) return false;
        if (promo.StartDate.CompareTo(DateTimeOffset.UtcNow) > 0) return false;
        if (promo.EndDate.CompareTo(DateTimeOffset.UtcNow) < 0) return false;

        return true;
    }
}