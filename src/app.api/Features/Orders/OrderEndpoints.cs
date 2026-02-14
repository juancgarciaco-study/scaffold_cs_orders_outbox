using app.api.Abstracts;

namespace app.api.Features.Orders;

public class OrderEndpoints : IEndpointGroup
{
    public static void MapGroup(WebApplication app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Api/Ordes");

        group.AddOrderCreate();

        //group.AddGetAllPeople();

    }
}
