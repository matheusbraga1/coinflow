namespace CoinFlow.Domain.Exceptions.Users;

public sealed class RefreshTokenNotFoundException : DomainExceptionBase
{
    public RefreshTokenNotFoundException() : base("Refresh token não encontrado") { }
}
