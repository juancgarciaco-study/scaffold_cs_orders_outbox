using app.api.Domain.Common;

namespace app.api.Domain;

public record OrderCreated(Guid OrderId, string CustomerName) : IDomainEvent;
