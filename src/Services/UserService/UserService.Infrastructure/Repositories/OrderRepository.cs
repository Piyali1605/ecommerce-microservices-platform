// OrderRepository.cs
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly UserDbContext _context;
    public OrderRepository(UserDbContext context) => _context = context;

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Orders.AsNoTracking().ToListAsync(ct);

    public async Task<Order> AddAsync(Order order, CancellationToken ct = default)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(ct);
        return order;
    }
}
