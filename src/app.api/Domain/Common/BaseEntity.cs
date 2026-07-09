using System.ComponentModel.DataAnnotations.Schema;

namespace app.api.Domain.Common;

public abstract class BaseEntity : IBaseDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
