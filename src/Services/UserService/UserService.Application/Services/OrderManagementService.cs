using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services;

public interface IOrderManagementService
{
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default);
    Task<OrderDto> PlaceOrderAsync(PlaceOrderRequest request, CancellationToken ct = default);
}

public class OrderManagementService : IOrderManagementService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUserRepository _userRepo;

    public OrderManagementService(IOrderRepository orderRepo, IProductRepository productRepo, IUserRepository userRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _orderRepo.GetAllAsync(ct)).Select(o => new OrderDto(o.Id, o.UserId, o.ProductId, o.Quantity, o.TotalAmount, o.Status, o.OrderedAt));

    public async Task<OrderDto> PlaceOrderAsync(PlaceOrderRequest request, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new KeyNotFoundException("User not found.");
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct)
            ?? throw new KeyNotFoundException("Product not found.");

        product.DeductStock(request.Quantity);
        await _productRepo.UpdateAsync(product, ct);

        var order = Order.Place(user.Id, product.Id, request.Quantity, product.Price);
        var saved = await _orderRepo.AddAsync(order, ct);

        return new OrderDto(saved.Id, saved.UserId, saved.ProductId, saved.Quantity, saved.TotalAmount, saved.Status, saved.OrderedAt);
    }
}
