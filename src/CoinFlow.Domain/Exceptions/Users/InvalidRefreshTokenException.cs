using CoinFlow.Domain.Exceptions.Base;

namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidRefreshTokenException : UnauthorizedException
{
    public InvalidRefreshTokenException() 
        : base("Token de atualização inválido ou expirado.") { }
}
