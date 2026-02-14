using app.api.Domain;
using app.api.Infrastructure;

namespace app.api.Features.Orders;

public static class OrderCreateEndpoint
{
    public static RouteHandlerBuilder AddOrderCreate(
        this RouteGroupBuilder group
    )
    {
        return group
            .MapPost("", async (ApplicationDbContext dbContext) =>
            {
                var order = Order.Create("Customer " + DateTime.UtcNow.Ticks);
                dbContext.Orders.Add(order);
                await dbContext.SaveChangesAsync();
                return Results.Ok(order);
            });
    }
}
