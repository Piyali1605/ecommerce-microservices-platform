using MassTransit;
using Microsoft.Extensions.Logging;
using UserService.Application.Events;
using UserService.Domain.Interfaces;

namespace UserService.Application.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IProductRepository _productRepo;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(IProductRepository productRepo, ILogger<OrderPlacedConsumer> logger)
    {
        _productRepo = productRepo;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("--> [RabbitMQ Consumer] Received OrderPlacedEvent for Order {OrderId}, Product {ProductId}, Qty {Qty}",
            msg.OrderId, msg.ProductId, msg.Quantity);

        var product = await _productRepo.GetByIdAsync(msg.ProductId, context.CancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found while processing Order {OrderId}", msg.ProductId, msg.OrderId);
            return;
        }

        product.DeductStock(msg.Quantity);
        await _productRepo.UpdateAsync(product, context.CancellationToken);

        _logger.LogInformation("--> [RabbitMQ Consumer] Stock updated for {ProductName}. Remaining stock: {Stock}",
            product.Name, product.StockQuantity);
    }
}
