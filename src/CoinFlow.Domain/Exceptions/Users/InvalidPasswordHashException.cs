using CoinFlow.Domain.Exceptions.Base;

namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidPasswordHashException : BadRequestException
{
    public InvalidPasswordHashException() : base("A senha criptografada não pode ser vazia") { }
}
