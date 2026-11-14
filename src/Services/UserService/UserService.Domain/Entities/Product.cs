namespace UserService.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }

    private Product() { }

    public static Product Create(string name, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.");
        if (price <= 0) throw new ArgumentException("Price must be greater than zero.");
        if (stockQuantity < 0) throw new ArgumentException("Stock cannot be negative.");

        return new Product { Name = name.Trim(), Price = price, StockQuantity = stockQuantity };
    }

    public void DeductStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        if (StockQuantity < quantity) throw new InvalidOperationException($"Insufficient stock for {Name}.");
        StockQuantity -= quantity;
    }
}
