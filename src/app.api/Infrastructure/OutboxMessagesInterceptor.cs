using System.Text.Json;
using app.api.Domain;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace app.api.Infrastructure;

public class OutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var outboxMessages = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(x => x.Entity)
            .SelectMany(aggregate =>
            {
                var domainEvents = aggregate.DomainEvents;
                var messages = domainEvents.Select(domainEvent => new OutboxMessage
                {
                    MessageId = Ulid.NewUlid(),
                    OccurredOnUtc = DateTime.UtcNow,
                    MessageType = domainEvent.GetType().Name,
                    Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    JsonContent = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
                }).ToList();

                aggregate.ClearDomainEvents();

                return messages;
            })
            .ToList();

        context.Set<OutboxMessage>().AddRange(outboxMessages);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
