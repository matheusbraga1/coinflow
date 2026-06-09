namespace CoinFlow.Domain.Exceptions.Base;

public abstract class NotFoundException : DomainExceptionBase
{
    protected NotFoundException(string message) : base(message) { }
}
