using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services;

public interface IProductManagementService
{
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
}

public class ProductManagementService : IProductManagementService
{
    private readonly IProductRepository _repo;
    public ProductManagementService(IProductRepository repo) => _repo = repo;

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _repo.GetAllAsync(ct)).Select(p => new ProductDto(p.Id, p.Name, p.Price, p.StockQuantity));

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var saved = await _repo.AddAsync(Product.Create(request.Name, request.Price, request.StockQuantity), ct);
        return new ProductDto(saved.Id, saved.Name, saved.Price, saved.StockQuantity);
    }
}
