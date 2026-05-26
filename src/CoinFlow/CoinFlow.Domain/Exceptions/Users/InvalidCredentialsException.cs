using CoinFlow.Domain.Exceptions;

namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidCredentialsException : DomainExceptionBase
{
    public InvalidCredentialsException() 
        : base("Email ou senha inválidos.") { }
}
