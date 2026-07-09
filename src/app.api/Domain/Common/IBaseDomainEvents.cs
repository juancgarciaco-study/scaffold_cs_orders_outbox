namespace app.api.Domain.Common;

public interface IBaseDomainEvents
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();

    void AddDomainEvent(IDomainEvent domainEvent);
}
