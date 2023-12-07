using KrMicro.Core.Services;
using KrMicro.Orders.Infrastructure;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.Services;

public interface IPromoService : IBaseService<Promo>
{
}

public class PromoRepositoryService : BaseRepositoryService<Promo, OrderDbContext>, IPromoService
{
    public PromoRepositoryService(OrderDbContext dataContext) : base(dataContext)
    {
    }
}