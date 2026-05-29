namespace CoinFlow.Domain.Exceptions.Base;

public abstract class BadRequestException : DomainExceptionBase
{
    protected BadRequestException(string message) : base(message) { }
}
