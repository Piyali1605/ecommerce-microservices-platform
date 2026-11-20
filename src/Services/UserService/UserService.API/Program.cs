using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using UserService.Application.Consumers;
using UserService.Application.Services;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connStr = builder.Configuration.GetConnectionString("DefaultConnection")!;

// 1. Register 3 Isolated DbContexts (Modular Monolith Data Ownership)
builder.Services.AddDbContext<UserDbContext>(opt => opt.UseSqlServer(connStr));
builder.Services.AddDbContext<ProductDbContext>(opt => opt.UseSqlServer(connStr));
builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseSqlServer(connStr));

// 2. Register Repositories & Application Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductManagementService, ProductManagementService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderManagementService, OrderManagementService>();

// 3. Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("StrongPass123!");
        });

        // Automatically creates queues & binds consumers
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// 4. Create database & isolated schemas (users, products, orders)
using (var scope = app.Services.CreateScope())
{
    var userDb = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    userDb.Database.EnsureCreated();

    var productCreator = (RelationalDatabaseCreator)scope.ServiceProvider.GetRequiredService<ProductDbContext>().Database.GetService<IDatabaseCreator>();
    try { productCreator.CreateTables(); } catch { /* Schema/tables already exist */ }

    var orderCreator = (RelationalDatabaseCreator)scope.ServiceProvider.GetRequiredService<OrderDbContext>().Database.GetService<IDatabaseCreator>();
    try { orderCreator.CreateTables(); } catch { /* Schema/tables already exist */ }
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();
app.Run();
