using CoinFlow.Domain.Exceptions.Base;

namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidEmailException : BadRequestException
{
    public InvalidEmailException(string attemptedValue)
        : base($"Email inválido: '{attemptedValue}'") { }
}
