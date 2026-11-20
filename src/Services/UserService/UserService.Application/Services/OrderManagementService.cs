using MassTransit;
using UserService.Application.DTOs;
using UserService.Application.Events;
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
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderManagementService(IOrderRepository orderRepo, IPublishEndpoint publishEndpoint)
    {
        _orderRepo = orderRepo;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _orderRepo.GetAllAsync(ct)).Select(o => new OrderDto(o.Id, o.UserId, o.ProductId, o.Quantity, o.TotalAmount, o.Status, o.OrderedAt));

    public async Task<OrderDto> PlaceOrderAsync(PlaceOrderRequest request, CancellationToken ct = default)
    {
        // 1. Save Order strictly inside OrderDbContext
        var order = Order.Place(request.UserId, request.ProductId, request.Quantity, request.UnitPrice);
        var saved = await _orderRepo.AddAsync(order, ct);

        // 2. Publish asynchronous integration event to RabbitMQ
        await _publishEndpoint.Publish(new OrderPlacedEvent(
            saved.Id, saved.UserId, saved.ProductId, saved.Quantity, saved.TotalAmount, saved.OrderedAt
        ), ct);

        return new OrderDto(saved.Id, saved.UserId, saved.ProductId, saved.Quantity, saved.TotalAmount, saved.Status, saved.OrderedAt);
    }
}
