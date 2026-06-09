using CoinFlow.Domain.Exceptions.Base;

namespace CoinFlow.Domain.Exceptions.Users;

public sealed class RefreshTokenNotFoundException : NotFoundException
{
    public RefreshTokenNotFoundException() : base("Token de atualização não encontrado") { }
}
