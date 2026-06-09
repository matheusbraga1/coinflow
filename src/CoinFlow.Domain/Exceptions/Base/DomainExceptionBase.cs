namespace CoinFlow.Domain.Exceptions.Base;

public abstract class DomainExceptionBase : Exception
{
    protected DomainExceptionBase(string message) : base(message) { }
}
