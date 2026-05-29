using CoinFlow.Domain.Exceptions.Base;

namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidUsernameException : BadRequestException
{
    public InvalidUsernameException(string message) : base(message) { }
}
