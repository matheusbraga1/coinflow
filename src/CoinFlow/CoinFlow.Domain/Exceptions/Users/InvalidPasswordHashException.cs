namespace CoinFlow.Domain.Exceptions.Users;

public sealed class InvalidPasswordHashException : DomainExceptionBase
{
    public InvalidPasswordHashException() : base("A senha criptografada não pode ser vazia") { }
}
