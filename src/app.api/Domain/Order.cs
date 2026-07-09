using app.api.Domain.Common;

namespace app.api.Domain;

public class Order : BaseEntity
{
    private Order() { } // For EF Core
    internal long Id { get; init; }

    public Ulid OrderId { get; private init; }

    public string CustomerName { get; private init; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public static Order Create(string customerName)
    {
        var order = new Order { OrderId = Ulid.NewUlid(), CustomerName = customerName, CreatedAt = DateTime.UtcNow };

        order.AddDomainEvent(new OrderCreated(order.OrderId.ToGuid(), order.CustomerName));

        return order;
    }
}
