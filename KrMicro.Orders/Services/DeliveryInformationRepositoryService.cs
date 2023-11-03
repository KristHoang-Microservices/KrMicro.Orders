using KrMicro.Core.Services;
using KrMicro.Orders.Infrastructure;
using KrMicro.Orders.Models;

namespace KrMicro.Orders.Services;

public interface IDeliveryInformationService : IBaseService<DeliveryInformation>
{
}

public class DeliveryInformationRepositoryService : BaseRepositoryService<DeliveryInformation, OrderDbContext>, IDeliveryInformationService
{
    public DeliveryInformationRepositoryService(OrderDbContext dataContext) : base(dataContext)
    {
    }
}