namespace UserService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; } = "Confirmed";
    public DateTime OrderedAt { get; private set; } = DateTime.UtcNow;

    private Order() { }

    public static Order Place(Guid userId, Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0) throw new ArgumentException("Order quantity must be at least 1.");
        return new Order
        {
            UserId = userId,
            ProductId = productId,
            Quantity = quantity,
            TotalAmount = unitPrice * quantity
        };
    }
}
