namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidUsernameException : DomainExceptionBase
{
    public InvalidUsernameException(string message) : base(message) { }
}
