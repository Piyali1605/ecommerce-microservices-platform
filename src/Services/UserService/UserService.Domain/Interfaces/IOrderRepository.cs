using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
    Task<Order> AddAsync(Order order, CancellationToken ct = default);
}
