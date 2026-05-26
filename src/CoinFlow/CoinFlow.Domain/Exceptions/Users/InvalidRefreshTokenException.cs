namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidRefreshTokenException : DomainExceptionBase
{
    public InvalidRefreshTokenException() 
        : base("Token de atualização inválido ou expirado.") { }
}
