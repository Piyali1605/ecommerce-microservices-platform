namespace UserService.Application.DTOs;

public record ProductDto(Guid Id, string Name, decimal Price, int StockQuantity);
public record CreateProductRequest(string Name, decimal Price, int StockQuantity);

public record OrderDto(Guid Id, Guid UserId, Guid ProductId, int Quantity, decimal TotalAmount, string Status, DateTime OrderedAt);
public record PlaceOrderRequest(Guid UserId, Guid ProductId, int Quantity, decimal UnitPrice);

