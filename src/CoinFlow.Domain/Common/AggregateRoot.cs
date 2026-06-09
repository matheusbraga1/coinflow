namespace CoinFlow.Domain.Common;

public abstract class AggregateRoot : Entity
{
    public readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(Guid id) : base(id) { }

    protected AggregateRoot() { }

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    protected void ClearDomainEvents() => _domainEvents.Clear();
}

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
