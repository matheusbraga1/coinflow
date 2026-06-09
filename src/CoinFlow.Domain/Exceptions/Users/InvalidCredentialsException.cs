using CoinFlow.Domain.Exceptions.Base;

namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidCredentialsException : UnauthorizedException
{
    public InvalidCredentialsException() 
        : base("Email ou senha inválidos.") { }
}
