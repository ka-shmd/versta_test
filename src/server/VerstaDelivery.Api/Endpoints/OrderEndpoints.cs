using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VerstaDelivery.Api.Data;
using VerstaDelivery.Api.DTOs;
using VerstaDelivery.Api.Models;
using VerstaDelivery.Api.Services;

namespace VerstaDelivery.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/orders").WithTags("Orders");

        group.MapPost("/", CreateOrder);
        group.MapGet("/", GetOrders);
        group.MapGet("/{orderNumber}", GetOrderByNumber).WithName("GetOrderByNumber");

        return builder;
    }

    private static async Task<Results<CreatedAtRoute<OrderDetails>, ValidationProblem>> CreateOrder(CreateOrderRequest request, IOrderNumberGenerator numberGenerator,
        AppDbContext context, ILoggerFactory loggerFactory, IValidator<CreateOrderRequest> validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var order = new Order
        {
            SenderCity = request.SenderCity,
            SenderAddress = request.SenderAddress,
            RecipientCity = request.RecipientCity,
            RecipientAddress = request.RecipientAddress,
            Weight = request.Weight,
            PickupDate = request.PickupDate,
            CreatedAt = DateTime.UtcNow
        };

        var logger = loggerFactory.CreateLogger(nameof(OrderEndpoints));

        for (int i = 0; i < 2; i++)
        {
            var orderNumber = numberGenerator.Generate();
            order.OrderNumber = orderNumber;
            context.Orders.Add(order);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                logger.LogWarning("Коллизия номера заказа {OrderNumber} на попытке {Attempt}.", orderNumber, i + 1);

                context.Entry(order).State = EntityState.Detached;
                if (i == 1)
                {
                    logger.LogError(ex, "Множественная коллизия номера заказа. Номер заказа {OrderNumber}", orderNumber);
                    throw;
                }
            }
        }

        logger.LogInformation("Заказ создан. Номер заказа: {OrderNumber}", order.OrderNumber);

        return TypedResults.CreatedAtRoute(order.ToDetails(), "GetOrderByNumber",
            new {orderNumber = order.OrderNumber});
    }

    private static async Task<Ok<PagedOrdersResponse>> GetOrders(AppDbContext context, CancellationToken cancellationToken,
        int page = 1, int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var orders = await context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ThenByDescending(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderSummary(o.OrderNumber, o.SenderCity, o.SenderAddress, o.RecipientCity, o.RecipientAddress, o.Weight, o.PickupDate))
            .ToArrayAsync(cancellationToken);

        var count = await context.Orders.CountAsync(cancellationToken);

        var response = new PagedOrdersResponse(orders, page, pageSize, (int) Math.Ceiling(count / (double) pageSize));

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<OrderDetails>, NotFound>> GetOrderByNumber(string orderNumber,
        AppDbContext context, CancellationToken cancellationToken)
    {
        var order = await context.Orders.AsNoTracking().SingleOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);

        if (order is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(order.ToDetails());
    }
}
