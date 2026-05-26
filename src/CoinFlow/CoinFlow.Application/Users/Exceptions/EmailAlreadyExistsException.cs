namespace CoinFlow.Application.Users.Exceptions;

public class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException(string email)
        : base($"Email '{email}' já está registrado.") { }
}
