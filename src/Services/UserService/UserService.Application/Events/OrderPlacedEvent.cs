namespace UserService.Application.Events;

public record OrderPlacedEvent(
    Guid OrderId,
    Guid UserId,
    Guid ProductId,
    int Quantity,
    decimal TotalAmount,
    DateTime OrderedAt
);
