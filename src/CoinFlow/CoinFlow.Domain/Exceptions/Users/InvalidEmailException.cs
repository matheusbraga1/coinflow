namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidEmailException : DomainExceptionBase
{
    public InvalidEmailException(string attemptedValue)
        : base($"Email inválido: '{attemptedValue}'") { }
}
