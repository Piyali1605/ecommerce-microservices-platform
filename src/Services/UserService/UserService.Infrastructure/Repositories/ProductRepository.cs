// ProductRepository.cs
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly UserDbContext _context;
    public ProductRepository(UserDbContext context) => _context = context;

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Products.AsNoTracking().ToListAsync(ct);

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Products.FindAsync(new object[] { id }, ct);

    public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(ct);
    }
}
