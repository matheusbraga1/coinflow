namespace CoinFlow.Domain.Exceptions.Base;

public abstract class ConflictException : DomainExceptionBase
{
    protected ConflictException(string message) : base(message) { }
}
