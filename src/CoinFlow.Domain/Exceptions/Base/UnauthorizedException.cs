namespace CoinFlow.Domain.Exceptions.Base;

public abstract class UnauthorizedException : DomainExceptionBase
{
    protected UnauthorizedException(string message) : base(message) { }
}
