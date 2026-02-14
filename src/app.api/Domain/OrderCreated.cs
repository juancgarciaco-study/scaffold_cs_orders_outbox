namespace app.api.Domain;

public record OrderCreated(Guid OrderId, string CustomerName) : IDomainEvent;
