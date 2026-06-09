using CoinFlow.Domain.Exceptions.Base;

namespace CoinFlow.Domain.Exceptions.Users;

public sealed class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid userId) 
        : base($"Usuário com ID: {userId} não encontrado.") { }
}
